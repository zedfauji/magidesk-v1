-- =====================================================================
-- Test Execution Tracking Schema
-- =====================================================================
-- Purpose: Track E2E test execution history for analysis and flaky test detection
-- Created: 2026-03-10
-- Requirements: 17.4, 19.4 from E2E Testing Comprehensive Scenarios spec
-- =====================================================================

-- =====================================================================
-- Table: test_executions
-- =====================================================================
-- Stores historical test execution records with timing, results, and environment info
CREATE TABLE IF NOT EXISTS test_executions (
    execution_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    test_name VARCHAR(500) NOT NULL,
    test_category VARCHAR(100) NOT NULL,
    test_priority VARCHAR(10) NOT NULL,
    started_at TIMESTAMP NOT NULL DEFAULT NOW(),
    completed_at TIMESTAMP,
    duration_ms INTEGER,
    result VARCHAR(20) NOT NULL, -- 'Passed', 'Failed', 'Skipped'
    failure_reason TEXT,
    stack_trace TEXT,
    machine_name VARCHAR(100),
    os_version VARCHAR(100),
    framework_version VARCHAR(50),
    created_at TIMESTAMP NOT NULL DEFAULT NOW()
);

-- =====================================================================
-- Indexes for test_executions
-- =====================================================================
-- Index for querying test history by name
CREATE INDEX IF NOT EXISTS idx_test_executions_name 
    ON test_executions(test_name);

-- Index for querying tests by execution time
CREATE INDEX IF NOT EXISTS idx_test_executions_started_at 
    ON test_executions(started_at);

-- Index for filtering by test result
CREATE INDEX IF NOT EXISTS idx_test_executions_result 
    ON test_executions(result);

-- =====================================================================
-- Table: test_artifacts
-- =====================================================================
-- Stores references to test failure artifacts (screenshots, UI trees, DB snapshots, process state)
CREATE TABLE IF NOT EXISTS test_artifacts (
    artifact_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    execution_id UUID NOT NULL REFERENCES test_executions(execution_id) ON DELETE CASCADE,
    artifact_type VARCHAR(50) NOT NULL, -- 'Screenshot', 'UITree', 'DatabaseSnapshot', 'ProcessState'
    file_path VARCHAR(1000) NOT NULL,
    file_size_bytes BIGINT,
    created_at TIMESTAMP NOT NULL DEFAULT NOW()
);

-- =====================================================================
-- Indexes for test_artifacts
-- =====================================================================
-- Index for querying artifacts by execution
CREATE INDEX IF NOT EXISTS idx_test_artifacts_execution_id 
    ON test_artifacts(execution_id);

-- =====================================================================
-- View: flaky_tests
-- =====================================================================
-- Detects tests that fail intermittently (failure rate between 10% and 90%)
-- Requires minimum 10 executions in last 30 days
CREATE OR REPLACE VIEW flaky_tests AS
SELECT 
    test_name,
    COUNT(*) as total_executions,
    SUM(CASE WHEN result = 'Failed' THEN 1 ELSE 0 END) as failure_count,
    CAST(SUM(CASE WHEN result = 'Failed' THEN 1 ELSE 0 END) AS FLOAT) / COUNT(*) as failure_rate,
    MAX(started_at) as last_execution,
    AVG(duration_ms) as avg_duration_ms
FROM test_executions
WHERE started_at > NOW() - INTERVAL '30 days'
GROUP BY test_name
HAVING COUNT(*) >= 10 AND 
       CAST(SUM(CASE WHEN result = 'Failed' THEN 1 ELSE 0 END) AS FLOAT) / COUNT(*) BETWEEN 0.1 AND 0.9;

-- =====================================================================
-- Comments for documentation
-- =====================================================================
COMMENT ON TABLE test_executions IS 'Historical test execution records for E2E test tracking and analysis';
COMMENT ON TABLE test_artifacts IS 'References to test failure artifacts (screenshots, UI trees, database snapshots, process state)';
COMMENT ON VIEW flaky_tests IS 'Detects tests with intermittent failures (10-90% failure rate) over last 30 days with minimum 10 executions';

COMMENT ON COLUMN test_executions.execution_id IS 'Unique identifier for test execution';
COMMENT ON COLUMN test_executions.test_name IS 'Fully qualified test name (namespace.class.method)';
COMMENT ON COLUMN test_executions.test_category IS 'Test category: FinancialSafety, OperationalIntegrity, Stability';
COMMENT ON COLUMN test_executions.test_priority IS 'Test priority: P0 (critical), P1 (operational), P2 (stability)';
COMMENT ON COLUMN test_executions.started_at IS 'Test execution start timestamp';
COMMENT ON COLUMN test_executions.completed_at IS 'Test execution completion timestamp';
COMMENT ON COLUMN test_executions.duration_ms IS 'Test execution duration in milliseconds';
COMMENT ON COLUMN test_executions.result IS 'Test result: Passed, Failed, Skipped';
COMMENT ON COLUMN test_executions.failure_reason IS 'Human-readable failure reason (null if passed)';
COMMENT ON COLUMN test_executions.stack_trace IS 'Exception stack trace (null if passed)';
COMMENT ON COLUMN test_executions.machine_name IS 'Machine name where test executed';
COMMENT ON COLUMN test_executions.os_version IS 'Operating system version';
COMMENT ON COLUMN test_executions.framework_version IS '.NET framework version';

COMMENT ON COLUMN test_artifacts.artifact_id IS 'Unique identifier for artifact';
COMMENT ON COLUMN test_artifacts.execution_id IS 'Reference to test execution';
COMMENT ON COLUMN test_artifacts.artifact_type IS 'Artifact type: Screenshot, UITree, DatabaseSnapshot, ProcessState';
COMMENT ON COLUMN test_artifacts.file_path IS 'File system path to artifact';
COMMENT ON COLUMN test_artifacts.file_size_bytes IS 'Artifact file size in bytes';

-- =====================================================================
-- End of migration
-- =====================================================================
