const fs = require('fs');
const path = require('path');
const https = require('https');

// Configuration
const CONFIG_PATH = path.join(process.env.USERPROFILE, '.config', 'moltbook', 'credentials.json');
const API_BASE = 'https://www.moltbook.com/api/v1';

// Helpers
function loadCredentials() {
    try {
        const data = fs.readFileSync(CONFIG_PATH, 'utf8');
        return JSON.parse(data);
    } catch (e) {
        console.error("Error loading credentials:", e.message);
        process.exit(1);
    }
}

async function apiCall(endpoint, method = 'GET', body = null) {
    const creds = loadCredentials();
    const headers = {
        'Authorization': `Bearer ${creds.api_key}`,
        'Content-Type': 'application/json'
    };

    const options = {
        method,
        headers,
    };

    if (body) {
        options.body = JSON.stringify(body);
    }

    try {
        const response = await fetch(`${API_BASE}${endpoint}`, options);
        if (!response.ok) {
            const text = await response.text();
            throw new Error(`API Error ${response.status}: ${text}`);
        }
        return await response.json();
    } catch (error) {
        console.error("Network Error:", error.message);
        process.exit(1);
    }
}

// Commands
async function getHotPosts(limit = 10) {
    console.log(`Fetching top ${limit} hot posts...`);
    const data = await apiCall(`/posts?sort=hot&limit=${limit}`);

    if (data.posts) {
        data.posts.forEach(post => {
            console.log(`\n[${post.id}] @${post.author_name} (${post.upvotes} ups)`);
            console.log(`Title: ${post.title}`);
            console.log(`"${post.content.substring(0, 100)}${post.content.length > 100 ? '...' : ''}"`);
        });
    } else {
        console.log("No posts found or unexpected format.");
    }
}

async function createPost(title, content) {
    console.log("Creating post...");
    const data = await apiCall('/posts', 'POST', { title, content });
    console.log("Success! Post ID:", data.id);
}

async function replyToPost(postId, content) {
    console.log(`Replying to ${postId}...`);
    const data = await apiCall(`/posts/${postId}/comments`, 'POST', { content });
    console.log("Success! Comment ID:", data.id);
}

// Main CLI
const args = process.argv.slice(2);
const command = args[0];

(async () => {
    try {
        switch (command) {
            case 'hot':
                await getHotPosts(args[1] || 10);
                break;
            case 'post':
                // node client.js post "Title" "Content"
                await createPost(args[1], args[2]);
                break;
            case 'reply':
                // node client.js reply <id> "Content"
                await replyToPost(args[1], args[2]);
                break;
            case 'test':
                const creds = loadCredentials();
                console.log(`Testing connection for ${creds.agent_name}...`);
                const test = await apiCall('/posts?limit=1');
                console.log("Connection Successful! ✅");
                break;
            default:
                console.log("Usage: node moltbook_client.js [hot|post|reply|test]");
        }
    } catch (e) {
        console.error("Execution failed:", e.message);
    }
})();
