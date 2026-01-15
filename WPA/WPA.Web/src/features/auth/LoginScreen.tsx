
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { authService } from '../../services';

export const LoginScreen = () => {
    const navigate = useNavigate();
    const [pin, setPin] = useState('');
    const [error, setError] = useState<string | null>(null);

    const handleNumberClick = (num: string) => {
        if (pin.length < 4) {
            setPin(prev => prev + num);
            setError(null);
        }
    };

    const handleClear = () => {
        setPin('');
        setError(null);
    };

    const handleBackspace = () => {
        setPin(prev => prev.slice(0, -1));
    };

    const handleLogin = async () => {
        try {
            await authService.login(pin);
            navigate('/tables');
        } catch (err) {
            setError('Invalid PIN');
            setPin('');
        }
    };

    return (
        <div className="bg-background-light dark:bg-background-dark text-white min-h-screen flex flex-col overflow-hidden font-display select-none">
            {/* Background Decorations */}
            <div className="fixed -top-10 left-1/2 -translate-x-1/2 opacity-5 pointer-events-none">
                <span className="material-symbols-outlined text-[120px] text-white/20">sports_handball</span>
            </div>
            <div className="fixed bottom-[-100px] left-[-100px] size-[400px] bg-primary/5 rounded-full blur-[120px] pointer-events-none"></div>
            <div className="fixed top-[20%] right-[-50px] size-[300px] bg-emerald-green/5 rounded-full blur-[100px] pointer-events-none"></div>

            {/* Header */}
            <header className="flex items-center justify-between px-8 py-6 border-b border-white/5 relative z-10 shrink-0">
                <div className="flex items-center gap-3">
                    <div className="size-8 text-primary">
                        <svg width="32" height="32" fill="none" viewBox="0 0 48 48" xmlns="http://www.w3.org/2000/svg">
                            <path clipRule="evenodd" d="M47.2426 24L24 47.2426L0.757355 24L24 0.757355L47.2426 24ZM12.2426 21H35.7574L24 9.24264L12.2426 21Z" fill="currentColor" fillRule="evenodd"></path>
                        </svg>
                    </div>
                    <h2 className="text-xl font-bold tracking-tight uppercase">Magidesk<span className="text-primary">POS</span></h2>
                </div>
                <div className="flex items-center gap-8">
                    <div className="text-right">
                        <div className="text-3xl font-light tracking-widest text-white/90">21:42</div>
                        <div className="text-[10px] uppercase tracking-widest text-white/40 font-bold">Terminal ID: T-08 (Lounge)</div>
                    </div>
                    <div className="flex gap-2">
                        <button className="size-12 rounded-lg bg-charcoal-surface flex items-center justify-center text-white/60 hover:text-white border border-white/10 transition-colors">
                            <span className="material-symbols-outlined">settings</span>
                        </button>
                        <button className="size-12 rounded-lg bg-charcoal-surface flex items-center justify-center text-white/60 hover:text-white border border-white/10 transition-colors">
                            <span className="material-symbols-outlined">help_outline</span>
                        </button>
                    </div>
                </div>
            </header>

            {/* Main Content */}
            <main className="flex-1 flex flex-col items-center justify-center pb-20 relative z-10">
                <div className="text-center mb-12">
                    <h1 className="text-white tracking-[0.2em] text-4xl font-bold leading-tight uppercase mb-2">Staff Access</h1>
                    <p className="text-white/50 text-sm font-normal uppercase tracking-widest">
                        {error ? <span className="text-red-500 animate-pulse font-bold">{error}</span> : "Enter authentication code to unlock terminal"}
                    </p>
                </div>

                {/* PIN Display */}
                <div className="flex justify-center gap-4 mb-10">
                    {[0, 1, 2, 3].map((i) => {
                        const hasDigit = i < pin.length;
                        return (
                            <div key={i} className={`w-16 h-20 rounded-xl bg-charcoal-surface border-2 flex items-center justify-center text-3xl font-bold transition-all duration-200 ${hasDigit
                                ? 'border-primary/40 text-primary shadow-[0_0_20px_rgba(43,189,238,0.2)]'
                                : 'border-white/10'
                                }`}>
                                {hasDigit ? (
                                    <span>{pin[i]}</span>
                                ) : (
                                    <div className="w-3 h-3 rounded-full bg-white/10"></div>
                                )}
                            </div>
                        );
                    })}
                </div>

                {/* Keypad */}
                <div className="w-full max-w-md bg-charcoal-surface p-8 rounded-2xl border border-white/5 shadow-2xl">
                    <div className="grid grid-cols-3 gap-4 mb-6">
                        {[1, 2, 3, 4, 5, 6, 7, 8, 9].map((num) => (
                            <button
                                key={num}
                                onClick={() => handleNumberClick(num.toString())}
                                className="h-20 rounded-xl bg-background-dark border border-white/5 flex items-center justify-center text-2xl font-bold transition-all hover:bg-white/5 active:scale-95 active:bg-primary active:text-background-dark active:border-primary"
                            >
                                {num}
                            </button>
                        ))}

                        <button
                            onClick={handleBackspace}
                            className="h-20 rounded-xl bg-background-dark border border-white/5 flex items-center justify-center text-white/40 transition-all hover:bg-white/5 active:scale-95"
                        >
                            <span className="material-symbols-outlined text-3xl">backspace</span>
                        </button>

                        <button
                            onClick={() => handleNumberClick('0')}
                            className="h-20 rounded-xl bg-background-dark border border-white/5 flex items-center justify-center text-2xl font-bold transition-all hover:bg-white/5 active:scale-95 active:bg-primary active:text-background-dark active:border-primary"
                        >
                            0
                        </button>

                        <button
                            onClick={handleClear}
                            className="h-20 rounded-xl bg-background-dark border border-white/5 flex items-center justify-center text-white/40 transition-all hover:bg-white/5 uppercase text-xs font-bold tracking-widest active:scale-95"
                        >
                            Clear
                        </button>
                    </div>

                    <button
                        onClick={handleLogin}
                        className="w-full h-20 bg-emerald-green hover:bg-emerald-green/90 text-background-dark rounded-xl flex items-center justify-center gap-3 transition-colors group active:scale-[0.98]"
                    >
                        <span className="text-xl font-extrabold uppercase tracking-[0.2em]">Start Shift</span>
                        <span className="material-symbols-outlined text-3xl transition-transform group-hover:translate-x-1">play_arrow</span>
                    </button>
                </div>

                {/* Footer Info */}
                <div className="mt-12 flex items-center gap-6 text-white/30 text-[10px] uppercase tracking-[0.3em] font-medium">
                    <div className="flex items-center gap-2">
                        <div className="size-1.5 rounded-full bg-emerald-green animate-pulse"></div>
                        Server Online
                    </div>
                    <div className="w-px h-3 bg-white/10"></div>
                    <div>Build 4.2.0-stable</div>
                    <div className="w-px h-3 bg-white/10"></div>
                    <div>v01.2024-HOTFIX</div>
                </div>
            </main>
        </div>
    );
};
