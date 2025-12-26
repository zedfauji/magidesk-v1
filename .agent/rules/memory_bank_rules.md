# Memory Bank Usage Rules

## Core Principle
The Memory Bank (located in `.gemini/antigravity/brain/...`) is the persistent long-term memory of the project. It MUST be kept up-to-date to ensure context remains across sessions.

## Mandatory Workflow
1.  **Start of Session**: Read [activeContext.md](file:///C:/Users/giris/.gemini/antigravity/brain/de13f5fa-ac56-468c-b54a-d49fbb90981f/activeContext.md) and [projectbrief.md](file:///C:/Users/giris/.gemini/antigravity/brain/de13f5fa-ac56-468c-b54a-d49fbb90981f/projectbrief.md) immediately after reading [task.md](file:///C:/Users/giris/.gemini/antigravity/brain/de13f5fa-ac56-468c-b54a-d49fbb90981f/task.md). This is non-negotiable.
2.  **Phase Completion**: When a major task or phase (e.g., "Phase 3.1") is completed, update [progress.md](file:///C:/Users/giris/.gemini/antigravity/brain/de13f5fa-ac56-468c-b54a-d49fbb90981f/progress.md) and [activeContext.md](file:///C:/Users/giris/.gemini/antigravity/brain/de13f5fa-ac56-468c-b54a-d49fbb90981f/activeContext.md).
3.  **Architecture Change**: If you introduce a new pattern (e.g., "Strategy Pattern for Tax"), update [systemPatterns.md](file:///C:/Users/giris/.gemini/antigravity/brain/de13f5fa-ac56-468c-b54a-d49fbb90981f/systemPatterns.md).
4.  **Tech Change**: If you add a library or change a tool, update [techContext.md](file:///C:/Users/giris/.gemini/antigravity/brain/de13f5fa-ac56-468c-b54a-d49fbb90981f/techContext.md).

## File Definitions
- **[projectbrief.md](file:///C:/Users/giris/.gemini/antigravity/brain/de13f5fa-ac56-468c-b54a-d49fbb90981f/projectbrief.md)**: The immutable core mission. (Read-Only mostly).
- **[activeContext.md](file:///C:/Users/giris/.gemini/antigravity/brain/de13f5fa-ac56-468c-b54a-d49fbb90981f/activeContext.md)**: The current session scratchpad. (Read/Write frequently).
- **[progress.md](file:///C:/Users/giris/.gemini/antigravity/brain/de13f5fa-ac56-468c-b54a-d49fbb90981f/progress.md)**: The high-level checklist of delivered value. (Write on completion).
- **[systemPatterns.md](file:///C:/Users/giris/.gemini/antigravity/brain/de13f5fa-ac56-468c-b54a-d49fbb90981f/systemPatterns.md)**: The architectural decision log. (Write on design change).

## Prohibited Actions
- ❌ Do NOT store ephemeral task details in [projectbrief.md](file:///C:/Users/giris/.gemini/antigravity/brain/de13f5fa-ac56-468c-b54a-d49fbb90981f/projectbrief.md).
- ❌ Do NOT delete history from [progress.md](file:///C:/Users/giris/.gemini/antigravity/brain/de13f5fa-ac56-468c-b54a-d49fbb90981f/progress.md) (append only).
- ❌ Do NOT ignore the Memory Bank when starting a new chat.
