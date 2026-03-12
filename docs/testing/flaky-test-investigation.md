# Flaky Test Investigation Guide

## Overview

This guide helps identify and remediate flaky tests using the test execution tracking database.

## Querying Flaky Tests

### View Flaky Tests

```sql
SELECT * FROM flaky_tests
WHERE failure_rate > 0.1 AND failure_rate < 0.9
ORDER BY failure_rate DESC;
```

### Test Execution History

```sql
SELECT test_name, result, started_at, duration_ms, failure_reason
FROM test_executions
WHERE test_name = 'YourTestName'
ORDER BY started_at DESC
LIMIT 20;
```

## Common Causes

1. **Timing Issues**: Tests depend on specific timing or delays
   - Solution: Use WaitHelpers with proper timeout configuration

2. **State Pollution**: Tests don't properly clean up state
   - Solution: Verify database reset and application restart

3. **External Dependencies**: Tests depend on external services
   - Solution: Mock or stub external dependencies

4. **Race Conditions**: Concurrent operations cause intermittent failures
   - Solution: Add proper synchronization

## Remediation Steps

1. Review test execution history to identify failure patterns
2. Analyze failure reasons and stack traces
3. Reproduce failure locally with increased logging
4. Apply appropriate fix based on root cause
5. Monitor test for 20+ executions to verify fix
