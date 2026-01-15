/** @type {import('tailwindcss').Config} */
export default {
    content: [
        "./index.html",
        "./src/**/*.{js,ts,jsx,tsx}",
    ],
    darkMode: "class",
    theme: {
        extend: {
            colors: {
                "primary": "#2bbdee",
                "background-light": "#f6f7f8",
                "background-dark": "#101d22",
                "emerald-green": "#4CAF50",
                "charcoal-surface": "#1c2427",
                "card-dark": "#1c2427",
            },
            fontFamily: {
                "display": ["Space Grotesk", "sans-serif"],
                "sans": ["Space Grotesk", "sans-serif"], // Set as default sans too for now
            },
        },
    },
    plugins: [],
}
