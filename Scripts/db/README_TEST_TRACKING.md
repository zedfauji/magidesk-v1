# Test Execution Tracking Schema

## Overview

The test execution tracking schema provides historical tracking of E2E test executions for the Magidesk POS application. This schema enables:

- Historical test execution analysis
- Flaky test detection (tests that fail intermittently)
- Test performance monitoring
- Failure artifact management (screenshots, UI trees, database snapshots, process state)

## Schema Components

### Tables

#### test_executions
Stores historical test execution records with timing, results, and environment information.

**Columns:**
- `execution_id` (UUID, PK): Unique identifier for test execution
- `test_name` (VARCHAR(500)): Fully qualified test name (namespace.class.method)
- `test_category` (VARCHAR(100)): Test category (FinancialSafety, OperationalIntegrity, Stability)
- `test_priority` (VARCHAR(10)): Test priority (P0, P1, P2)
- `started_at` (TIMESTAMP): Test execution start timestamp
- `completed_at` (TIMESTAMP): Test execution completion timestamp
- `duration_ms` (INTEGER): Test execution duration in milliseconds
- `result` (VARCHAR(20)): Test result (Passed, Failed, Skipped)
- `failure_reason` (TEXT): Human-readable failure reason (null if passed)
- `stack_trace` (TEXT): Exception stack trace (null if passed)
- `machine_name` (VARCHAR(100)): Machine name where test executed
- `os_version` (VARCHAR(100)): Operating system version
- `framework_version` (VARCHAR(50)): .NET framework version
- `created_at` (TIMESTAMP): Record creation timestamp

**Indexes:**
- `idx_test_executions_name`: Query test history by name
- `idx_test_executions_started_at`: Query tests by execution time
- `idx_test_executions_result`: Filter by test result

#### test_artifacts
Stores references to test failure artifacts captured during test execution.

**Columns:**
- `artifact_id` (UUID, PK): Unique identifier for artifact
- `execution_id` (UUID, FK): Reference to test execution
- `artifact_type` (VARCHAR(50)): Artifact type (Screenshot, UITree, DatabaseSnapshot, ProcessState)
- `file_path` (VARCHAR(1000)): File system path to artifact
- `file_size_bytes` (BIGINT): Artifact file size in bytes
- `created_at` (TIMESTAMP): Record creation timestamp

**Indexes:**
- `idx_test_artifacts_execution_id`: Query artifacts by execution

### Views

#### flaky_tests
Detects tests that fail intermittently (failure rate between 10% and 90%) over the last 30 days with minimum 10 executions.

**Columns:**
- `test_name`: Test name
- `total_executions`: Total number of executions in last 30 days
- `failure_count`: Number of failed executions
- `failure_rate`: Failure rate (0.0 to 1.0)
- `last_execution`: Timestamp of most recent execution
- `avg_duration_ms`: Average execution duration in milliseconds

## Installation

### Manual Application

To apply this migration manually using psql:

```powershell
# Set connection parameters
$DbHost = "localhost"
$DbPort = "5432"
$DbUser = "postgres"
$DbName = "magidesk_prod"

# Apply migration
psql -h $DbHost -p $DbPort -U $DbUser -d $DbName -f "Scripts/db/02_test_execution_tracking.sql"
```

### Automated Application

The migration will be applied automatically when running the database setup scripts:

```powershell
.\Scripts\install_db.ps1 -DbHost localhost -DbUser postgres -DbPass yourpassword
```

## Usage Examples

### Query Test History

```sql
-- Get last 10 executions for a specific test
SELECT 
    execution_id,
    started_at,
    duration_ms,
    result,
    failure_reason
FROM test_executions
WHERE test_name = 'Magidesk.Tests.Workflows.AuthenticationTests.ValidPinLogin'
ORDER BY started_at DESC
LIMIT 10;
```

### Identify Flaky Tests

```sql
-- Get all flaky tests
SELECT 
    test_name,
    total_executions,
    failure_count,
    ROUND(failure_rate * 100, 2) as failure_percentage,
    last_execution,
    ROUND(avg_duration_ms) as avg_duration_ms
FROM flaky_tests
ORDER BY failure_rate DESC;
```

### Get Test Execution Statistics

```sql
-- Get test statistics for last 7 days
SELECT 
    test_category,
    test_priority,
    COUNT(*) as total_executions,
    SUM(CASE WHEN result = 'Passed' THEN 1 ELSE 0 END) as passed,
    SUM(CASE WHEN result = 'Failed' THEN 1 ELSE 0 END) as failed,
    SUM(CASE WHEN result = 'Skipped' THEN 1 ELSE 0 END) as skipped,
    AVG(duration_ms) as avg_duration_ms
FROM test_executions
WHERE started_at > NOW() - INTERVAL '7 days'
GROUP BY test_category, test_priority
ORDER BY test_category, test_priority;
```

### Query Test Artifacts

```sql
-- Get all artifacts for a failed test execution
SELECT 
    te.test_name,
    te.started_at,
    te.result,
    ta.artifact_type,
    ta.file_path,
    ta.file_size_bytes
FROM test_executions te
JOIN test_artifacts ta ON te.execution_id = ta.execution_id
WHERE te.execution_id = 'your-execution-id-here'
ORDER BY ta.artifact_type;
```

## Requirements Validation

This schema satisfies the following requirements from the E2E Testing Comprehensive Scenarios spec:

- **Requirement 17.4**: Track test execution time for performance monitoring
- **Requirement 19.4**: Maintain test execution history in PostgreSQL database

## Maintenance

### Data Retention

Consider implementing a data retention policy to prevent unbounded growth:

```sql
-- Delete test executions older than 90 days
DELETE FROM test_executions
WHERE started_at < NOW() - INTERVAL '90 days';
```

### Performance Monitoring

Monitor index usage and query performance:

```sql
-- Check index usage
SELECT 
    schemaname,
    tablename,
    indexname,
    idx_scan,
    idx_tup_read,
    idx_tup_fetch
FROM pg_stat_user_indexes
WHERE tablename IN ('test_executions', 'test_artifacts')
ORDER BY idx_scan DESC;
```

## Related Documentation

- E2E Testing Comprehensive Scenarios Spec: `Magidesk/.kiro/specs/e2e-testing-comprehensive-scenarios/`
- Test Execution Guide: `docs/testing/e2e-test-execution-guide.md` (to be created in Task 41.1)
- Flaky Test Investigation Guide: `docs/testing/flaky-test-investigation.md` (to be created in Task 41.3)
