const fs = require('fs');
const path = require('path');

const CONFIG_PATH = path.join(process.env.USERPROFILE, '.config', 'moltbook', 'credentials.json');
const creds = JSON.parse(fs.readFileSync(CONFIG_PATH, 'utf8'));

console.log("Checking status for:", creds.agent_name);

fetch('https://www.moltbook.com/api/v1/agents/status', {
    headers: { 'Authorization': `Bearer ${creds.api_key}` }
})
    .then(r => r.json())
    .then(console.log)
    .catch(console.error);
