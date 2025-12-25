# MCP Tools Usage Rules

## Context7 MCP Tool

### Purpose
- Get latest documentation for libraries and frameworks
- Access up-to-date API references
- Get code examples and best practices
- Stay current with library updates

### When to Use
- **REQUIRE**: Before implementing new library features
- **REQUIRE**: When documentation is unclear
- **REQUIRE**: When checking for breaking changes
- **REQUIRE**: When learning new library APIs
- **REQUIRE**: When verifying best practices

### Usage Pattern
1. Resolve library ID using `resolve-library-id`
2. Get library docs using `get-library-docs`
3. Use `mode=code` for API references and code examples
4. Use `mode=info` for conceptual guides and architecture
5. Reference documentation when implementing

### Examples
- **EF Core**: Get latest EF Core documentation before configuring entities
- **CommunityToolkit.Mvvm**: Get latest MVVM patterns before implementing ViewModels
- **FluentValidation**: Get latest validation patterns
- **Polly**: Get latest resilience patterns
- **Dapper**: Get latest query patterns (if used)

### Prohibited
- ❌ Skip documentation lookup for new libraries
- ❌ Use outdated documentation
- ❌ Implement without checking latest patterns
- ❌ Ignore breaking changes

### Required
- ✅ Check documentation before using new library features
- ✅ Use latest patterns from documentation
- ✅ Verify compatibility with our .NET version
- ✅ Follow library best practices

## PostgreSQL MCP Tool

### Purpose
- Execute SQL queries for database operations
- Analyze database schema
- Check database health
- Optimize queries
- Manage database structure

### When to Use
- **REQUIRE**: Database setup and migrations
- **REQUIRE**: Query optimization
- **REQUIRE**: Database health checks
- **REQUIRE**: Schema analysis
- **REQUIRE**: Index recommendations
- **ALLOW**: Development and testing queries

### Usage Rules
- **Database**: `magidesk_pos`
- **Schema**: `magidesk` (use this schema)
- **Connection**: Local passwordless PostgreSQL
- **Queries**: Use for setup, analysis, optimization
- **Migrations**: Use EF Core migrations for schema changes (not direct SQL)

### Allowed Operations
- ✅ Execute read-only queries for analysis
- ✅ Check database health
- ✅ Analyze query performance
- ✅ Get index recommendations
- ✅ List schemas and tables
- ✅ Analyze database structure

### Prohibited Operations
- ❌ Direct schema modifications (use EF Core migrations)
- ❌ Production data modifications
- ❌ Bypassing EF Core for regular operations
- ❌ Manual table creation (use migrations)

### Integration with EF Core
- Use MCP tool for analysis and optimization
- Use EF Core for all schema changes
- Use MCP tool to verify migrations
- Use MCP tool for query analysis

## Sequential Thinking MCP Tool

### Purpose
- Break down complex problems
- Plan multi-step solutions
- Analyze complex scenarios
- Design with room for revision
- Maintain context over multiple steps

### When to Use
- **REQUIRE**: Complex domain problems
- **REQUIRE**: Architecture decisions
- **REQUIRE**: Multi-step implementations
- **REQUIRE**: Problem analysis
- **REQUIRE**: Design planning

### Usage Scenarios
- Designing complex domain workflows
- Planning payment processing logic
- Analyzing discount calculation rules
- Designing state machines
- Planning refactoring efforts

### Best Practices
- Start with initial estimate of thoughts needed
- Adjust total thoughts as understanding deepens
- Question or revise previous thoughts
- Branch or backtrack when needed
- Generate solution hypothesis
- Verify hypothesis based on steps

### Prohibited
- ❌ Skip thinking for complex problems
- ❌ Jump to solution without analysis
- ❌ Ignore alternative approaches

### Required
- ✅ Use for complex domain logic
- ✅ Use for architecture decisions
- ✅ Document thinking process
- ✅ Revise approach when needed

## Memory Bank MCP Tool

### Purpose
- Store and retrieve project knowledge
- Maintain project context and decisions
- Document architectural decisions
- Store domain knowledge
- Keep project memory persistent

### When to Use
- **REQUIRE**: Store important architectural decisions
- **REQUIRE**: Document domain model decisions
- **REQUIRE**: Store project brief and context
- **REQUIRE**: Maintain design decision history
- **ALLOW**: Store implementation patterns
- **ALLOW**: Document workflows and processes

### Usage Pattern
1. Use `get_memory_bank_structure` to understand structure
2. Use `generate_memory_bank_template` for new files
3. Use `analyze_project_summary` for project analysis
4. Store important decisions and context
5. Retrieve stored knowledge when needed

### Memory Bank Files
- **projectbrief.md**: Project overview and goals
- **architecture.md**: Architectural decisions
- **domain.md**: Domain model decisions
- **implementation.md**: Implementation patterns
- **decisions.md**: Design decisions log
- **workflows.md**: Business workflows

### Best Practices
- Update memory bank when making important decisions
- Store rationale for architectural choices
- Document domain model evolution
- Keep memory bank current
- Use for onboarding new team members

### Prohibited
- ❌ Store sensitive data (secrets, passwords)
- ❌ Store temporary or trivial information
- ❌ Skip updating when decisions change
- ❌ Duplicate information already in documentation

### Required
- ✅ Store architectural decisions
- ✅ Document domain model rationale
- ✅ Keep project context current
- ✅ Use for knowledge persistence

## MCP Tool Integration

### Workflow
1. Use Sequential Thinking for complex problems
2. Use Context7 for library documentation
3. Use PostgreSQL MCP for database operations
4. Use Memory Bank for knowledge persistence
5. Integrate findings into implementation

### Documentation
- Document MCP tool usage in code comments if relevant
- Reference documentation sources
- Keep implementation aligned with latest patterns
- Store important decisions in Memory Bank

## Prohibited Practices

### NEVER:
- Skip documentation lookup for new libraries
- Use outdated library patterns
- Bypass MCP tools when they would help
- Ignore tool recommendations
- Store sensitive data in Memory Bank
- Skip updating Memory Bank when decisions change

### ALWAYS:
- Use Context7 for library documentation
- Use PostgreSQL MCP for database analysis
- Use Sequential Thinking for complex problems
- Use Memory Bank for important decisions
- Follow latest best practices from tools
- Keep Memory Bank current
