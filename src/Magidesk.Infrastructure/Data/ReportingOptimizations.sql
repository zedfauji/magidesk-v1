-- Reporting Performance Optimization Script
-- Creates materialized views and indexes for improved report performance
-- This script should be run after the main database schema is created

-- =====================================================
-- MATERIALIZED VIEWS FOR REPORT PERFORMANCE
-- =====================================================

-- Daily Sales Summary Materialized View
-- Aggregates daily sales data for faster daily report generation
CREATE MATERIALIZED VIEW IF NOT EXISTS daily_sales_summary AS
SELECT 
    DATE(t.created_at) as sale_date,
    SUM(t.total_amount) as total_sales,
    SUM(CASE 
        WHEN ol.menu_item_id IS NULL AND t.table_session_id IS NOT NULL 
        THEN ol.total_price 
        ELSE 0 
    END) as time_sales,
    SUM(CASE 
        WHEN ol.menu_item_id IS NOT NULL 
        THEN ol.total_price 
        ELSE 0 
    END) as product_sales,
    SUM(t.tax_amount) as total_tax,
    SUM(t.gratuity_amount) as total_gratuity,
    COUNT(DISTINCT t.id) as transaction_count,
    COUNT(DISTINCT t.customer_id) as customer_count,
    AVG(t.total_amount) as average_ticket_size
FROM tickets t
LEFT JOIN order_lines ol ON t.id = ol.ticket_id
WHERE t.status = 'PAID'
GROUP BY DATE(t.created_at);

-- Hourly Sales Breakdown Materialized View
-- Provides hourly sales patterns for performance analysis
CREATE MATERIALIZED VIEW IF NOT EXISTS hourly_sales_summary AS
SELECT 
    DATE(t.created_at) as sale_date,
    EXTRACT(HOUR FROM t.created_at) as sale_hour,
    SUM(t.total_amount) as total_sales,
    COUNT(DISTINCT t.id) as transaction_count,
    COUNT(DISTINCT t.customer_id) as customer_count,
    AVG(t.total_amount) as average_ticket_size
FROM tickets t
WHERE t.status = 'PAID'
GROUP BY DATE(t.created_at), EXTRACT(HOUR FROM t.created_at);

-- Table Utilization Summary Materialized View
-- Aggregates table session data for utilization reports
CREATE MATERIALIZED VIEW IF NOT EXISTS table_utilization_summary AS
SELECT 
    DATE(ts.start_time) as session_date,
    ts.table_id,
    tb.table_number,
    tt.name as table_type,
    COUNT(*) as session_count,
    SUM(EXTRACT(EPOCH FROM (ts.end_time - ts.start_time))/3600) as total_hours,
    AVG(EXTRACT(EPOCH FROM (ts.end_time - ts.start_time))/3600) as avg_session_hours,
    SUM(COALESCE(ts.time_charge_amount, 0)) as total_time_revenue
FROM table_sessions ts
INNER JOIN tables tb ON ts.table_id = tb.id
INNER JOIN table_types tt ON tb.table_type_id = tt.id
WHERE ts.end_time IS NOT NULL
GROUP BY DATE(ts.start_time), ts.table_id, tb.table_number, tt.name;

-- Member Activity Summary Materialized View
-- Aggregates member visit and spending patterns
CREATE MATERIALIZED VIEW IF NOT EXISTS member_activity_summary AS
SELECT 
    DATE(t.created_at) as activity_date,
    t.customer_id,
    c.first_name,
    c.last_name,
    c.email,
    COUNT(DISTINCT t.id) as visit_count,
    SUM(t.total_amount) as total_spent,
    AVG(t.total_amount) as avg_ticket_size,
    MAX(t.created_at) as last_visit_date,
    SUM(CASE WHEN ts.id IS NOT NULL THEN 1 ELSE 0 END) as table_sessions
FROM tickets t
INNER JOIN customers c ON t.customer_id = c.id
LEFT JOIN table_sessions ts ON t.table_session_id = ts.id
WHERE t.status = 'PAID' AND c.is_member = true
GROUP BY DATE(t.created_at), t.customer_id, c.first_name, c.last_name, c.email;

-- Server Performance Summary Materialized View
-- Aggregates server sales and performance metrics
CREATE MATERIALIZED VIEW IF NOT EXISTS server_performance_summary AS
SELECT 
    DATE(t.created_at) as shift_date,
    t.server_id,
    u.first_name as server_first_name,
    u.last_name as server_last_name,
    COUNT(DISTINCT t.id) as transaction_count,
    SUM(t.total_amount) as total_sales,
    SUM(t.gratuity_amount) as total_tips,
    AVG(t.total_amount) as avg_ticket_size,
    AVG(CASE WHEN t.total_amount > 0 THEN (t.gratuity_amount / t.total_amount) * 100 ELSE 0 END) as avg_tip_percentage
FROM tickets t
INNER JOIN users u ON t.server_id = u.id
WHERE t.status = 'PAID' AND t.server_id IS NOT NULL
GROUP BY DATE(t.created_at), t.server_id, u.first_name, u.last_name;

-- =====================================================
-- PERFORMANCE INDEXES FOR REPORT QUERIES
-- =====================================================

-- Indexes for ticket-based queries
CREATE INDEX IF NOT EXISTS idx_tickets_created_at_status ON tickets(created_at, status);
CREATE INDEX IF NOT EXISTS idx_tickets_date_status_server ON tickets(DATE(created_at), status, server_id);
CREATE INDEX IF NOT EXISTS idx_tickets_customer_date ON tickets(customer_id, created_at) WHERE status = 'PAID';
CREATE INDEX IF NOT EXISTS idx_tickets_table_session_date ON tickets(table_session_id, created_at) WHERE status = 'PAID';

-- Indexes for table session queries
CREATE INDEX IF NOT EXISTS idx_table_sessions_date_range ON table_sessions(start_time, end_time);
CREATE INDEX IF NOT EXISTS idx_table_sessions_table_date ON table_sessions(table_id, DATE(start_time));
CREATE INDEX IF NOT EXISTS idx_table_sessions_end_time ON table_sessions(end_time) WHERE end_time IS NOT NULL;

-- Indexes for order line queries
CREATE INDEX IF NOT EXISTS idx_order_lines_ticket_menu_item ON order_lines(ticket_id, menu_item_id);
CREATE INDEX IF NOT EXISTS idx_order_lines_menu_item_date ON order_lines(menu_item_id, created_at);

-- Indexes for payment queries
CREATE INDEX IF NOT EXISTS idx_payments_ticket_type_date ON payments(ticket_id, payment_type, created_at);
CREATE INDEX IF NOT EXISTS idx_payments_date_type_status ON payments(DATE(created_at), payment_type, status);

-- Indexes for customer queries
CREATE INDEX IF NOT EXISTS idx_customers_member_last_visit ON customers(is_member, last_visit_date);
CREATE INDEX IF NOT EXISTS idx_customers_email_member ON customers(email, is_member) WHERE is_member = true;

-- Indexes for cash session queries
CREATE INDEX IF NOT EXISTS idx_cash_sessions_date_status ON cash_sessions(DATE(opened_at), status);
CREATE INDEX IF NOT EXISTS idx_cash_sessions_terminal_date ON cash_sessions(terminal_id, DATE(opened_at));

-- Indexes for shift queries
CREATE INDEX IF NOT EXISTS idx_shifts_date_status ON shifts(DATE(start_time), status);
CREATE INDEX IF NOT EXISTS idx_shifts_user_date ON shifts(user_id, DATE(start_time));

-- =====================================================
-- MATERIALIZED VIEW REFRESH FUNCTIONS
-- =====================================================

-- Function to refresh all reporting materialized views
-- This should be called periodically (e.g., every hour or at end of day)
CREATE OR REPLACE FUNCTION refresh_reporting_views()
RETURNS void AS $$
BEGIN
    -- Refresh all materialized views concurrently for better performance
    REFRESH MATERIALIZED VIEW CONCURRENTLY daily_sales_summary;
    REFRESH MATERIALIZED VIEW CONCURRENTLY hourly_sales_summary;
    REFRESH MATERIALIZED VIEW CONCURRENTLY table_utilization_summary;
    REFRESH MATERIALIZED VIEW CONCURRENTLY member_activity_summary;
    REFRESH MATERIALIZED VIEW CONCURRENTLY server_performance_summary;
    
    -- Log the refresh
    INSERT INTO system_logs (level, message, created_at)
    VALUES ('INFO', 'Reporting materialized views refreshed', NOW());
    
EXCEPTION
    WHEN OTHERS THEN
        -- Log any errors
        INSERT INTO system_logs (level, message, created_at)
        VALUES ('ERROR', 'Failed to refresh reporting views: ' || SQLERRM, NOW());
        RAISE;
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- UNIQUE INDEXES FOR MATERIALIZED VIEWS (for CONCURRENTLY refresh)
-- =====================================================

-- Unique indexes required for concurrent refresh
CREATE UNIQUE INDEX IF NOT EXISTS daily_sales_summary_unique_idx ON daily_sales_summary(sale_date);
CREATE UNIQUE INDEX IF NOT EXISTS hourly_sales_summary_unique_idx ON hourly_sales_summary(sale_date, sale_hour);
CREATE UNIQUE INDEX IF NOT EXISTS table_utilization_summary_unique_idx ON table_utilization_summary(session_date, table_id);
CREATE UNIQUE INDEX IF NOT EXISTS member_activity_summary_unique_idx ON member_activity_summary(activity_date, customer_id);
CREATE UNIQUE INDEX IF NOT EXISTS server_performance_summary_unique_idx ON server_performance_summary(shift_date, server_id);

-- =====================================================
-- AUTOMATED REFRESH SCHEDULE (Optional - requires pg_cron extension)
-- =====================================================

-- Uncomment the following lines if pg_cron extension is available
-- This will automatically refresh materialized views every hour

-- SELECT cron.schedule('refresh-reporting-views', '0 * * * *', 'SELECT refresh_reporting_views();');

-- =====================================================
-- PERFORMANCE MONITORING VIEWS
-- =====================================================

-- View to monitor materialized view sizes and last refresh times
CREATE OR REPLACE VIEW materialized_view_stats AS
SELECT 
    schemaname,
    matviewname,
    matviewowner,
    tablespace,
    hasindexes,
    ispopulated,
    definition
FROM pg_matviews 
WHERE matviewname LIKE '%_summary';

-- View to monitor index usage statistics
CREATE OR REPLACE VIEW report_index_usage AS
SELECT 
    schemaname,
    tablename,
    indexname,
    idx_tup_read,
    idx_tup_fetch,
    idx_scan
FROM pg_stat_user_indexes 
WHERE indexname LIKE 'idx_%'
ORDER BY idx_scan DESC;

-- =====================================================
-- CLEANUP AND MAINTENANCE
-- =====================================================

-- Function to analyze tables after bulk operations
CREATE OR REPLACE FUNCTION analyze_reporting_tables()
RETURNS void AS $$
BEGIN
    ANALYZE tickets;
    ANALYZE table_sessions;
    ANALYZE order_lines;
    ANALYZE payments;
    ANALYZE customers;
    ANALYZE cash_sessions;
    ANALYZE shifts;
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- COMMENTS FOR DOCUMENTATION
-- =====================================================

COMMENT ON MATERIALIZED VIEW daily_sales_summary IS 'Aggregated daily sales data for fast report generation';
COMMENT ON MATERIALIZED VIEW hourly_sales_summary IS 'Hourly sales breakdown for performance analysis';
COMMENT ON MATERIALIZED VIEW table_utilization_summary IS 'Table usage statistics for utilization reports';
COMMENT ON MATERIALIZED VIEW member_activity_summary IS 'Member visit and spending patterns';
COMMENT ON MATERIALIZED VIEW server_performance_summary IS 'Server sales and performance metrics';

COMMENT ON FUNCTION refresh_reporting_views() IS 'Refreshes all reporting materialized views';
COMMENT ON FUNCTION analyze_reporting_tables() IS 'Updates table statistics for query optimization';