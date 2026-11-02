<!DOCTYPE html>
<html lang="ja">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>CYBER SURVIVOR: Text Battle Royale</title>
    <!-- Tailwind CSS CDN -->
    <script src="https://cdn.tailwindcss.com"></script>
    <!-- FontAwesome for Icons -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css">
    <!-- Google Fonts -->
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
    <link href="https://fonts.googleapis.com/css2?family=Fira+Code:wght@400;600;700&family=Noto+Sans+JP:wght@400;700&display=swap" rel="stylesheet">

    <style>
        body {
            font-family: 'Noto Sans JP', 'Fira Code', monospace;
            background-color: #090d16;
            color: #d1d5db;
            user-select: none;
        }

        /* カスタムスクロールバー */
        ::-webkit-scrollbar {
            width: 6px;
        }
        ::-webkit-scrollbar-track {
            background: #0f172a;
        }
        ::-webkit-scrollbar-thumb {
            background: #334155;
            border-radius: 3px;
        }
        ::-webkit-scrollbar-thumb:hover {
            background: #475569;
        }

        /* 光るエフェクト */
        .glow-green {
            text-shadow: 0 0 8px rgba(34, 197, 94, 0.6);
        }
        .glow-red {
            text-shadow: 0 0 8px rgba(239, 68, 68, 0.6);
        }
        .glow-cyan {
            text-shadow: 0 0 8px rgba(6, 182, 212, 0.6);
        }

        /* ターミナル風パルス */
        .pulse-border {
            box-shadow: 0 0 15px rgba(6, 182, 212, 0.15);
            border: 1px solid rgba(6, 182, 212, 0.3);
        }

        .danger-pulse {
            animation: pulse-red 2s infinite;
        }

        @keyframes pulse-red {
            0%, 100% { border-color: rgba(239, 68, 68, 0.4); box-shadow: 0 0 10px rgba(239, 68, 68, 0.2); }
            50% { border-color: rgba(239, 68, 68, 0.9); box-shadow: 0 0 20px rgba(239, 68, 68, 0.5); }
        }

        /* 戦闘演出用アニメーション */
        @keyframes shake {
            0%, 100% { transform: translate(0, 0) rotate(0deg); }
            20% { transform: translate(-6px, 4px) rotate(-1deg); }
            40% { transform: translate(6px, -4px) rotate(1deg); }
            60% { transform: translate(-4px, -2px) rotate(-0.5deg); }
            80% { transform: translate(4px, 2px) rotate(0.5deg); }
        }

        .shake-effect {
            animation: shake 0.35s cubic-bezier(.36,.07,.19,.97) both;
        }

        @keyframes damage-pop {
            0% { opacity: 0; transform: translate(-50%, -20%) scale(0.5); }
            30% { opacity: 1; transform: translate(-50%, -70%) scale(1.3); }
            70% { opacity: 1; transform: translate(-50%, -100%) scale(1.0); }
            100% { opacity: 0; transform: translate(-50%, -130%) scale(0.8); }
        }

        .damage-pop-el {
            position: absolute;
            left: 50%;
            top: 50%;
            pointer-events: none;
            z-index: 60;
            font-weight: 900;
            font-family: 'Fira Code', monospace;
            animation: damage-pop 0.85s forwards ease-out;
            text-shadow: 0 0 10px rgba(0,0,0,0.9);
        }

        @keyframes flash-red {
            0% { opacity: 0.6; }
            100% { opacity: 0; }
        }

        .hit-flash {
            position: fixed;
            inset: 0;
            background-color: rgba(239, 68, 68, 0.3);
            pointer-events: none;
            z-index: 45;
            animation: flash-red 0.3s forwards ease-out;
        }

        .crit-flash {
            position: fixed;
            inset: 0;
            background-color: rgba(245, 158, 11, 0.35);
            pointer-events: none;
            z-index: 45;
            animation: flash-red 0.4s forwards ease-out;
        }

        /* 斬撃・銃撃ヒット時のスラッシュアニメーション */
        @keyframes slash-anim {
            0% {
                opacity: 0;
                transform: translate(-50%, -50%) rotate(-45deg) scaleX(0);
            }
            30% {
                opacity: 1;
                transform: translate(-50%, -50%) rotate(-45deg) scaleX(1.4);
            }
            100% {
                opacity: 0;
                transform: translate(-50%, -50%) rotate(-45deg) scaleX(1.8);
            }
        }

        .slash-effect {
            position: absolute;
            left: 50%;
            top: 45%;
            width: 140%;
            height: 4px;
            background: linear-gradient(90deg, transparent, #06b6d4, #ffffff, #06b6d4, transparent);
            box-shadow: 0 0 15px #06b6d4, 0 0 30px #ffffff;
            pointer-events: none;
            z-index: 55;
            animation: slash-anim 0.35s ease-out forwards;
        }

        .slash-effect-crit {
            position: absolute;
            left: 50%;
            top: 45%;
            width: 160%;
            height: 8px;
            background: linear-gradient(90deg, transparent, #f59e0b, #ffffff, #ef4444, transparent);
            box-shadow: 0 0 20px #f59e0b, 0 0 40px #ef4444;
            pointer-events: none;
            z-index: 55;
            animation: slash-anim 0.4s ease-out forwards;
        }
    </style>
</head>
<body class="min-h-screen bg-slate-950 flex flex-col overflow-y-auto lg:overflow-hidden">

    <!-- App Container -->
    <div id="app" class="flex flex-col flex-1 max-w-7xl w-full mx-auto p-2 sm:p-4 gap-3">

        <!-- HEADER / STATUS TOP BAR -->
        <header class="bg-slate-900/90 backdrop-blur border border-slate-800 rounded-xl p-3 flex flex-wrap items-center justify-between shadow-lg gap-2 shrink-0">
            <div class="flex items-center gap-3">
                <i class="fa-solid font-bold text-2xl text-cyan-400 fa-crosshairs glow-cyan"></i>
                <div>
                    <h1 class="text-base sm:text-lg font-bold tracking-wider text-cyan-400 glow-cyan leading-none">CYBER SURVIVOR</h1>
                    <span class="text-xs text-slate-500">ZONE-99 BATTLE ROYALE TERMINAL</span>
                </div>
            </div>

            <!-- Global Match Info -->
            <div class="flex items-center gap-4 text-xs sm:text-sm">
                <!-- BGM Control Button -->
                <button id="btn-toggle-bgm" class="bg-slate-950/80 hover:bg-slate-800 px-3 py-1.5 rounded-lg border border-cyan-500/40 text-cyan-400 flex items-center gap-2 transition">
                    <i id="bgm-icon" class="fa-solid fa-volume-xmark"></i>
                    <span id="bgm-status-text" class="font-mono text-xs">BGM: OFF</span>
                </button>
                <div class="bg-slate-950/80 px-3 py-1.5 rounded-lg border border-slate-800 flex items-center gap-2">
                    <i class="fa-solid fa-users text-emerald-400"></i>
                    <span>生存者: <strong id="header-alive" class="text-emerald-400 text-base">100</strong> / <span id="header-total">100</span></span>
                </div>
                <div class="bg-slate-950/80 px-3 py-1.5 rounded-lg border border-slate-800 flex items-center gap-2">
                    <i class="fa-solid fa-skull text-red-400"></i>
                    <span>撃破数: <strong id="header-kills" class="text-red-400 text-base">0</strong></span>
                </div>
                <div class="bg-slate-950/80 px-3 py-1.5 rounded-lg border border-slate-800 flex items-center gap-2">
                    <i class="fa-solid fa-stopwatch text-amber-400"></i>
                    <span>経過ターン: <strong id="header-turn" class="text-amber-400 text-base">1</strong></span>
                </div>
            </div>
        </header>

        <!-- START SCREEN MODAL OVERLAY -->
        <div id="start-screen" class="fixed inset-0 z-50 bg-slate-950/95 flex items-center justify-center p-4">
            <div class="bg-slate-900 border border-cyan-500/40 rounded-2xl p-6 sm:p-8 max-w-md w-full shadow-2xl space-y-6 pulse-border">
                <div class="text-center space-y-2">
                    <i class="fa-solid fa-radiation text-5xl text-cyan-400 glow-cyan animate-pulse"></i>
                    <h2 class="text-2xl font-bold text-cyan-400 tracking-wider">CYBER SURVIVOR</h2>
                    <p class="text-xs text-slate-400">100名による極限バトルロワイヤル。最後の1人を目指せ。</p>
                </div>

                <div class="space-y-4">
                    <div>
                        <label class="block text-xs font-semibold text-slate-400 mb-1">コードネーム（名前）</label>
                        <input type="text" id="player-name-input" value="Operator" maxlength="16" class="w-full bg-slate-950 border border-slate-700 rounded-lg px-3 py-2 text-cyan-300 focus:outline-none focus:border-cyan-500 font-mono text-sm">
                    </div>

                    <div class="grid grid-cols-2 gap-2">
                        <div>
                            <label class="block text-xs font-semibold text-slate-400 mb-1">戦闘スタイル（パーク）</label>
                            <select id="player-perk-input" class="w-full bg-slate-950 border border-slate-700 rounded-lg px-3 py-2 text-slate-200 focus:outline-none focus:border-cyan-500 text-sm">
                                <option value="assault">アサルト (攻撃力+15%, 初期ハンドガン)</option>
                                <option value="scout">スカウト (回避率+10%, 発見率UP)</option>
                                <option value="medic">メディック (回復+30%, 初期救急箱)</option>
                                <option value="tank">タフネス (最大HP+25, ダメージ10%減)</option>
                            </select>
                        </div>
                        <div>
                            <label class="block text-xs font-semibold text-slate-400 mb-1">参戦BOT人数</label>
                            <select id="game-difficulty-input" class="w-full bg-slate-950 border border-slate-700 rounded-lg px-3 py-2 text-slate-200 focus:outline-none focus:border-cyan-500 text-sm">
                                <option value="easy">10名 (クイックマッチ)</option>
                                <option value="normal" selected>50名 (スタンダード)</option>
                                <option value="hard">99名 (フルマッチ)</option>
                            </select>
                        </div>
                    </div>

                    <div class="bg-slate-950/60 p-3 rounded-lg border border-slate-800 text-xs text-slate-400 space-y-1">
                        <div class="text-amber-400 font-bold mb-1"><i class="fa-solid fa-play mr-1"></i>ゲーム開始手順</div>
                        <p>・下部の「START MATCH」ボタンを押すと即座に戦場へ降下します。</p>
                        <p>・タクティカルマップのセルをクリックするとエリアを簡単に選択・移動できます。</p>
                    </div>
                </div>

                <button id="btn-start-game" class="w-full py-4 bg-gradient-to-r from-cyan-600 to-blue-600 hover:from-cyan-500 hover:to-blue-500 text-white font-extrabold rounded-xl shadow-lg shadow-cyan-500/20 transition-all transform hover:scale-[1.02] active:scale-[0.98] flex items-center justify-center gap-2 text-base tracking-widest border border-cyan-400/30">
                    <i class="fa-solid fa-play text-lg"></i> START MATCH / 戦場降下
                </button>
            </div>
        </div>

        <!-- MAIN GAME PANEL (GRID LAYOUT) -->
        <main class="flex-1 grid grid-cols-1 lg:grid-cols-12 gap-3 min-h-0 overflow-hidden">

            <!-- LEFT COLUMN: Status & Inventory (4 cols) -->
            <section class="lg:col-span-4 flex flex-col gap-3 min-h-0 overflow-y-auto pr-1">
                
                <!-- Player Status Card -->
                <div class="bg-slate-900/80 border border-slate-800 rounded-xl p-3.5 space-y-3">
                    <div class="flex items-center justify-between border-b border-slate-800 pb-2">
                        <div class="flex items-center gap-2">
                            <i class="fa-solid fa-user-shield text-cyan-400"></i>
                            <span id="player-name-display" class="font-bold text-slate-200 text-sm">SURVIVOR</span>
                        </div>
                        <span id="player-perk-tag" class="text-[10px] bg-slate-800 text-cyan-400 px-2 py-0.5 rounded border border-cyan-500/30">Assault</span>
                    </div>

                    <!-- HP Bar -->
                    <div class="space-y-1">
                        <div class="flex justify-between text-xs font-semibold">
                            <span class="text-slate-400"><i class="fa-solid fa-heart text-red-500 mr-1"></i>体 力 (HP)</span>
                            <span id="hp-text" class="text-red-400">100 / 100</span>
                        </div>
                        <div class="w-full h-2.5 bg-slate-950 rounded-full overflow-hidden p-0.5 border border-slate-800">
                            <div id="hp-bar" class="h-full bg-gradient-to-r from-red-600 to-emerald-500 rounded-full transition-all duration-300" style="width: 100%;"></div>
                        </div>
                    </div>

                    <!-- Armor & Combat Stats -->
                    <div class="grid grid-cols-2 gap-2 text-xs">
                        <div class="bg-slate-950/80 p-2 rounded-lg border border-slate-800/80">
                            <div class="text-slate-500 text-[10px]"><i class="fa-solid fa-shield-halved text-blue-400 mr-1"></i>アーマー軽減</div>
                            <div id="status-armor" class="font-bold text-blue-400 text-sm mt-0.5">0%</div>
                        </div>
                        <div class="bg-slate-950/80 p-2 rounded-lg border border-slate-800/80">
                            <div class="text-slate-500 text-[10px]"><i class="fa-solid fa-gun text-amber-400 mr-1"></i>総攻撃力</div>
                            <div id="status-atk" class="font-bold text-amber-400 text-sm mt-0.5">10</div>
                        </div>
                    </div>
                </div>

                <!-- Inventory & Equipment Card -->
                <div class="bg-slate-900/80 border border-slate-800 rounded-xl p-3.5 space-y-3 flex-1 flex flex-col min-h-[200px]">
                    <h3 class="text-xs font-bold text-slate-400 tracking-wider flex items-center justify-between border-b border-slate-800 pb-2">
                        <span><i class="fa-solid fa-boxes-packing text-cyan-400 mr-1.5"></i>所持品・装備</span>
                        <span id="inventory-capacity" class="text-[10px] text-slate-500">0/6 Slots</span>
                    </h3>

                    <!-- Equipment slots -->
                    <div class="grid grid-cols-2 gap-2 text-xs">
                        <div class="bg-slate-950 p-2 rounded-lg border border-slate-800 flex items-center gap-2 overflow-hidden">
                            <i class="fa-solid fa-crosshairs text-amber-400 text-base shrink-0"></i>
                            <div class="overflow-hidden min-w-0">
                                <div class="text-[10px] text-slate-500">武器</div>
                                <div id="eq-weapon" class="font-semibold text-slate-200 truncate text-[11px]">素手 (ATK +0)</div>
                            </div>
                        </div>
                        <div class="bg-slate-950 p-2 rounded-lg border border-slate-800 flex items-center gap-2 overflow-hidden">
                            <i class="fa-solid fa-vest text-blue-400 text-base shrink-0"></i>
                            <div class="overflow-hidden min-w-0">
                                <div class="text-[10px] text-slate-500">防具</div>
                                <div id="eq-armor" class="font-semibold text-slate-200 truncate text-[11px]">なし (DEF 0%)</div>
                            </div>
                        </div>
                    </div>

                    <!-- Inventory list -->
                    <div id="inventory-list" class="space-y-1.5 overflow-y-auto flex-1 pr-1 min-h-[100px] max-h-[180px]">
                        <!-- Items rendered via JS -->
                    </div>
                </div>
            </section>

            <!-- CENTER/RIGHT COLUMN: Tactical Map & Control Console (8 cols) -->
            <section class="lg:col-span-8 flex flex-col gap-3 min-h-0">
                
                <!-- Tactical Map & Combat Area -->
                <div id="tactical-container" class="bg-slate-900/80 border border-slate-800 rounded-xl p-3.5 space-y-3 flex-1 flex flex-col min-h-0 relative">
                    <div class="flex items-center justify-between border-b border-slate-800 pb-2">
                        <span class="text-xs font-bold text-slate-400 tracking-wider"><i class="fa-solid fa-map-location-dot text-cyan-400 mr-1.5"></i>タクティカルマップ (3x3 GRID)</span>
                        <span id="ring-status" class="text-[11px] font-mono text-amber-400"><i class="fa-solid fa-circle-radiation mr-1"></i>安全地帯収縮まで: 4ターン</span>
                    </div>

                    <!-- Grid Map (3x3 Interactive) -->
                    <div class="grid grid-cols-3 gap-2 flex-1 min-h-[200px]">
                        <!-- 9 Grid cells dynamically updated -->
                        <div id="cell-0" onclick="clickCell(0)" class="bg-slate-950 border border-slate-800 rounded-xl p-2 flex flex-col justify-between cursor-pointer hover:border-cyan-500/50 transition relative overflow-hidden group min-h-[65px]"></div>
                        <div id="cell-1" onclick="clickCell(1)" class="bg-slate-950 border border-slate-800 rounded-xl p-2 flex flex-col justify-between cursor-pointer hover:border-cyan-500/50 transition relative overflow-hidden group min-h-[65px]"></div>
                        <div id="cell-2" onclick="clickCell(2)" class="bg-slate-950 border border-slate-800 rounded-xl p-2 flex flex-col justify-between cursor-pointer hover:border-cyan-500/50 transition relative overflow-hidden group min-h-[65px]"></div>
                        <div id="cell-3" onclick="clickCell(3)" class="bg-slate-950 border border-slate-800 rounded-xl p-2 flex flex-col justify-between cursor-pointer hover:border-cyan-500/50 transition relative overflow-hidden group min-h-[65px]"></div>
                        <div id="cell-4" onclick="clickCell(4)" class="bg-slate-950 border border-slate-800 rounded-xl p-2 flex flex-col justify-between cursor-pointer hover:border-cyan-500/50 transition relative overflow-hidden group min-h-[65px]"></div>
                        <div id="cell-5" onclick="clickCell(5)" class="bg-slate-950 border border-slate-800 rounded-xl p-2 flex flex-col justify-between cursor-pointer hover:border-cyan-500/50 transition relative overflow-hidden group min-h-[65px]"></div>
                        <div id="cell-6" onclick="clickCell(6)" class="bg-slate-950 border border-slate-800 rounded-xl p-2 flex flex-col justify-between cursor-pointer hover:border-cyan-500/50 transition relative overflow-hidden group min-h-[65px]"></div>
                        <div id="cell-7" onclick="clickCell(7)" class="bg-slate-950 border border-slate-800 rounded-xl p-2 flex flex-col justify-between cursor-pointer hover:border-cyan-500/50 transition relative overflow-hidden group min-h-[65px]"></div>
                        <div id="cell-8" onclick="clickCell(8)" class="bg-slate-950 border border-slate-800 rounded-xl p-2 flex flex-col justify-between cursor-pointer hover:border-cyan-500/50 transition relative overflow-hidden group min-h-[65px]"></div>
                    </div>

                    <!-- Action Controls Panel -->
                    <div id="normal-controls" class="grid grid-cols-2 sm:grid-cols-4 gap-2 pt-1">
                        <button id="btn-action-search" class="py-2.5 px-2 bg-slate-800 hover:bg-slate-700 text-slate-100 font-bold rounded-lg border border-slate-700 text-xs flex items-center justify-center gap-1.5 transition">
                            <i class="fa-solid fa-magnifying-glass text-cyan-400"></i> 物資探索
                        </button>
                        <button id="btn-action-rest" class="py-2.5 px-2 bg-slate-800 hover:bg-slate-700 text-slate-100 font-bold rounded-lg border border-slate-700 text-xs flex items-center justify-center gap-1.5 transition">
                            <i class="fa-solid fa-kit-medical text-emerald-400"></i> 休息・回復
                        </button>
                        <button id="btn-action-scan" class="py-2.5 px-2 bg-slate-800 hover:bg-slate-700 text-slate-100 font-bold rounded-lg border border-slate-700 text-xs flex items-center justify-center gap-1.5 transition">
                            <i class="fa-solid fa-radar text-amber-400"></i> 周囲索敵
                        </button>
                        <button id="btn-action-move" class="py-2.5 px-2 bg-cyan-900/50 hover:bg-cyan-800/60 text-cyan-200 font-bold rounded-lg border border-cyan-700/50 text-xs flex items-center justify-center gap-1.5 transition">
                            <i class="fa-solid fa-person-walking text-cyan-400"></i> エリア移動
                        </button>
                    </div>

                    <!-- Combat Controls Panel (Hidden by default) -->
                    <div id="combat-controls" class="hidden grid-cols-2 sm:grid-cols-4 gap-2 pt-1 border-t border-red-900/50">
                        <button id="btn-combat-attack" class="py-2.5 px-2 bg-red-900/70 hover:bg-red-800 text-red-100 font-extrabold rounded-lg border border-red-700 text-xs flex items-center justify-center gap-1.5 transition animate-pulse">
                            <i class="fa-solid fa-gun text-red-400"></i> 攻撃 (ATTACK)
                        </button>
                        <button id="btn-combat-defend" class="py-2.5 px-2 bg-slate-800 hover:bg-slate-700 text-slate-100 font-bold rounded-lg border border-slate-700 text-xs flex items-center justify-center gap-1.5 transition">
                            <i class="fa-solid fa-shield text-blue-400"></i> 防御・身隠し
                        </button>
                        <button id="btn-combat-item" class="py-2.5 px-2 bg-slate-800 hover:bg-slate-700 text-slate-100 font-bold rounded-lg border border-slate-700 text-xs flex items-center justify-center gap-1.5 transition">
                            <i class="fa-solid fa-hand-holding-medical text-emerald-400"></i> 回復使用
                        </button>
                        <button id="btn-combat-flee" class="py-2.5 px-2 bg-amber-950/60 hover:bg-amber-900 text-amber-200 font-bold rounded-lg border border-amber-800 text-xs flex items-center justify-center gap-1.5 transition">
                            <i class="fa-solid fa-person-running text-amber-400"></i> 撤退・逃走
                        </button>
                    </div>
                </div>

                <!-- Log Terminal & Kill Feed -->
                <div class="bg-slate-900/80 border border-slate-800 rounded-xl p-3 grid grid-cols-1 md:grid-cols-3 gap-3 min-h-[160px]">
                    <!-- Main Battle Log (2 cols) -->
                    <div class="md:col-span-2 flex flex-col min-h-0 h-40 md:h-auto">
                        <div class="text-[10px] font-bold text-slate-500 uppercase tracking-widest mb-1 flex items-center gap-1">
                            <i class="fa-solid fa-terminal text-cyan-400"></i> BATTLE LOG / ターミナルログ
                        </div>
                        <div id="log-terminal" class="flex-1 bg-slate-950 rounded-lg p-2.5 border border-slate-800/80 font-mono text-xs overflow-y-auto space-y-1">
                            <!-- JS Inject -->
                        </div>
                    </div>
                    <!-- Kill Feed (1 col) -->
                    <div class="flex flex-col min-h-0 h-32 md:h-auto">
                        <div class="text-[10px] font-bold text-slate-500 uppercase tracking-widest mb-1 flex items-center gap-1">
                            <i class="fa-solid fa-skull-crossbones text-red-400"></i> KILL FEED / 戦況全域ログ
                        </div>
                        <div id="kill-feed" class="flex-1 bg-slate-950 rounded-lg p-2 border border-slate-800/80 font-mono text-[11px] overflow-y-auto space-y-1">
                            <!-- JS Inject -->
                        </div>
                    </div>
                </div>

            </section>
        </main>
    </div>

    <!-- AREA MOVE MODAL OVERLAY -->
    <div id="move-modal" class="fixed inset-0 z-40 bg-slate-950/80 hidden items-center justify-center p-4">
        <div class="bg-slate-900 border border-slate-700 rounded-xl p-5 max-w-sm w-full space-y-4">
            <h3 class="text-sm font-bold text-cyan-400 flex items-center justify-between">
                <span><i class="fa-solid fa-map-location-dot mr-2"></i>移動先エリアを選択</span>
                <button id="btn-close-move" class="text-slate-400 hover:text-white"><i class="fa-solid fa-xmark"></i></button>
            </h3>
            <div id="move-list" class="space-y-2 max-h-60 overflow-y-auto">
                <!-- Move options -->
            </div>
        </div>
    </div>

    <!-- END GAME MODAL OVERLAY -->
    <div id="end-screen" class="fixed inset-0 z-50 bg-slate-950/95 hidden items-center justify-center p-4">
        <div class="bg-slate-900 border border-slate-700 rounded-2xl p-6 sm:p-8 max-w-md w-full shadow-2xl text-center space-y-6 pulse-border">
            <div id="end-icon" class="text-6xl text-amber-400 glow-yellow">
                <i class="fa-solid fa-trophy"></i>
            </div>
            <div class="space-y-1">
                <h2 id="end-title" class="text-2xl font-extrabold tracking-wider text-amber-400 glow-cyan">VICTORY ROYALE!</h2>
                <p id="end-subtitle" class="text-xs text-slate-400">最後の1人として生存しました。</p>
            </div>

            <div class="bg-slate-950 p-4 rounded-xl border border-slate-800 grid grid-cols-2 gap-3 text-xs">
                <div>
                    <div class="text-slate-500">最終順位</div>
                    <div id="end-rank" class="text-amber-400 font-bold text-lg">#1 / 50</div>
                </div>
                <div>
                    <div class="text-slate-500">撃破数 (Kills)</div>
                    <div id="end-kills" class="text-red-400 font-bold text-lg">0</div>
                </div>
                <div>
                    <div class="text-slate-500">生存ターン</div>
                    <div id="end-turns" class="text-cyan-400 font-bold text-base">12</div>
                </div>
                <div>
                    <div class="text-slate-500">使用パーク</div>
                    <div id="end-perk" class="text-slate-200 font-bold text-base">Assault</div>
                </div>
            </div>

            <button id="btn-return-lobby" class="w-full py-3.5 bg-gradient-to-r from-slate-700 to-slate-800 hover:from-slate-600 hover:to-slate-700 text-slate-100 font-bold rounded-xl shadow-lg border border-slate-600 transition transform hover:scale-[1.02] flex items-center justify-center gap-2">
                <i class="fa-solid fa-house"></i> ロビー（スタート画面）へ戻る
            </button>
        </div>
    </div>

    <!-- GAME LOGIC SCRIPT -->
    <script>
        // --- CYBER BGM AUDIO ENGINE (Web Audio API - Ultra Cyberpunk Synthwave Engine) ---
        class CyberAudioEngine {
            constructor() {
                this.ctx = null;
                this.isPlaying = false;
                this.tempo = 128;
                this.step = 0;
                this.timer = null;
                // サイバーパンク・フューチャーダークスケール (F Minor / Db Major 緊迫感強化)
                this.bassNotes = [43.65, 43.65, 51.91, 43.65, 65.41, 58.27, 51.91, 48.99]; // F1, Ab1, F1, C2, Bb1, Ab1, G1
                this.leadNotes = [174.61, 207.65, 261.63, 311.13, 349.23, 311.13, 261.63, 207.65]; // F3, Ab3, C4, Eb4, F4
                this.arpeggioNotes = [349.23, 415.30, 523.25, 622.25, 698.46, 830.61, 698.46, 523.25]; // F4, Ab4, C5, Eb5, F5, Ab5
                this.subNotes = [21.83, 21.83, 25.96, 24.50]; // F0 Sub Bass (腹に響く最低域)
            }

            init() {
                if (!this.ctx) {
                    const AudioCtx = window.AudioContext || window.webkitAudioContext;
                    this.ctx = new AudioCtx();
                }
                if (this.ctx.state === 'suspended') {
                    this.ctx.resume();
                }
            }

            toggle() {
                this.init();
                if (this.isPlaying) {
                    this.stop();
                } else {
                    this.start();
                }
                return this.isPlaying;
            }

            start() {
                if (this.isPlaying) return;
                this.isPlaying = true;
                this.step = 0;
                const interval = (60 / this.tempo / 4) * 1000;
                this.timer = setInterval(() => this.playStep(), interval);
            }

            stop() {
                this.isPlaying = false;
                if (this.timer) {
                    clearInterval(this.timer);
                    this.timer = null;
                }
            }

            playStep() {
                if (!this.ctx || !this.isPlaying) return;

                const t = this.ctx.currentTime;
                const stepInBar = this.step % 16;
                const bar = Math.floor(this.step / 16);

                // 1. 重低音ディストーション・サイバーキック (Cyber Heavy Kick)
                if (stepInBar === 0 || stepInBar === 4 || stepInBar === 8 || stepInBar === 12 || (stepInBar === 15 && Math.random() < 0.4)) {
                    const osc = this.ctx.createOscillator();
                    const gain = this.ctx.createGain();

                    osc.type = 'sine';
                    osc.frequency.setValueAtTime(220, t);
                    osc.frequency.exponentialRampToValueAtTime(28, t + 0.16);

                    gain.gain.setValueAtTime(0.65, t);
                    gain.gain.exponentialRampToValueAtTime(0.001, t + 0.18);

                    osc.connect(gain);
                    gain.connect(this.ctx.destination);

                    osc.start(t);
                    osc.stop(t + 0.19);
                }

                // 2. メタルノイズ・インダストリアルスネア (Heavy Metal Snare + Noise)
                if (stepInBar === 4 || stepInBar === 12) {
                    const bufferSize = this.ctx.sampleRate * 0.15;
                    const buffer = this.ctx.createBuffer(1, bufferSize, this.ctx.sampleRate);
                    const data = buffer.getChannelData(0);
                    for (let i = 0; i < bufferSize; i++) {
                        data[i] = Math.random() * 2 - 1;
                    }

                    const noise = this.ctx.createBufferSource();
                    noise.buffer = buffer;

                    const noiseFilter = this.ctx.createBiquadFilter();
                    noiseFilter.type = 'bandpass';
                    noiseFilter.frequency.value = 1800;
                    noiseFilter.Q.value = 1.2;

                    const noiseGain = this.ctx.createGain();
                    noiseGain.gain.setValueAtTime(0.35, t);
                    noiseGain.gain.exponentialRampToValueAtTime(0.001, t + 0.15);

                    noise.connect(noiseFilter);
                    noiseFilter.connect(noiseGain);
                    noiseGain.connect(this.ctx.destination);

                    noise.start(t);
                }

                // 3. 高速16分音符 サイバーハイハット (Cyber Metallic Hat)
                if (stepInBar % 2 === 1 || stepInBar % 4 === 2) {
                    const bufferSize = this.ctx.sampleRate * 0.04;
                    const buffer = this.ctx.createBuffer(1, bufferSize, this.ctx.sampleRate);
                    const data = buffer.getChannelData(0);
                    for (let i = 0; i < bufferSize; i++) {
                        data[i] = Math.random() * 2 - 1;
                    }

                    const noise = this.ctx.createBufferSource();
                    noise.buffer = buffer;

                    const noiseFilter = this.ctx.createBiquadFilter();
                    noiseFilter.type = 'highpass';
                    noiseFilter.frequency.value = 9000;

                    const noiseGain = this.ctx.createGain();
                    const isOpen = (stepInBar === 6 || stepInBar === 14);
                    const vol = isOpen ? 0.09 : 0.05;
                    const dur = isOpen ? 0.07 : 0.025;

                    noiseGain.gain.setValueAtTime(vol, t);
                    noiseGain.gain.exponentialRampToValueAtTime(0.0001, t + dur);

                    noise.connect(noiseFilter);
                    noiseFilter.connect(noiseGain);
                    noiseGain.connect(this.ctx.destination);

                    noise.start(t);
                }

                // 4. 超低音サブドローン (Sub-Zero Bass)
                if (stepInBar % 4 === 0) {
                    const subFreq = this.subNotes[Math.floor(this.step / 8) % this.subNotes.length];
                    const subOsc = this.ctx.createOscillator();
                    const subGain = this.ctx.createGain();
                    
                    subOsc.type = 'sawtooth';
                    subOsc.frequency.setValueAtTime(subFreq, t);

                    const subFilter = this.ctx.createBiquadFilter();
                    subFilter.type = 'lowpass';
                    subFilter.frequency.value = 180;

                    subGain.gain.setValueAtTime(0.3, t);
                    subGain.gain.exponentialRampToValueAtTime(0.01, t + 0.4);

                    subOsc.connect(subFilter);
                    subFilter.connect(subGain);
                    subGain.connect(this.ctx.destination);

                    subOsc.start(t);
                    subOsc.stop(t + 0.42);
                }

                // 5. 凶暴なウルトラ・ソーウェーブベース (Aggressive Sawtooth Cyber Bass)
                if (stepInBar % 2 === 0) {
                    const bassFreq = this.bassNotes[Math.floor(this.step / 2) % this.bassNotes.length];
                    const osc1 = this.ctx.createOscillator();
                    const osc2 = this.ctx.createOscillator();
                    const gain = this.ctx.createGain();
                    const filter = this.ctx.createBiquadFilter();

                    osc1.type = 'sawtooth';
                    osc1.frequency.setValueAtTime(bassFreq, t);

                    // 厚みを生むデチューン (ピッチズレ)
                    osc2.type = 'sawtooth';
                    osc2.frequency.setValueAtTime(bassFreq * 1.012, t);

                    filter.type = 'lowpass';
                    const filterMod = 400 + (Math.sin(this.step / 4) + 1) * 800;
                    filter.frequency.setValueAtTime(filterMod, t);
                    filter.frequency.exponentialRampToValueAtTime(150, t + 0.16);
                    filter.Q.value = 6.0;

                    gain.gain.setValueAtTime(0.18, t);
                    gain.gain.exponentialRampToValueAtTime(0.001, t + 0.18);

                    osc1.connect(filter);
                    osc2.connect(filter);
                    filter.connect(gain);
                    gain.connect(this.ctx.destination);

                    osc1.start(t);
                    osc2.start(t);
                    osc1.stop(t + 0.19);
                    osc2.stop(t + 0.19);
                }

                // 6. アルペジオ・ネオンリフレイン (High-Speed Cyber Arpeggio)
                const arpFreq = this.arpeggioNotes[this.step % this.arpeggioNotes.length];
                const arpOsc = this.ctx.createOscillator();
                const arpGain = this.ctx.createGain();
                const arpFilter = this.ctx.createBiquadFilter();

                arpOsc.type = 'square';
                arpOsc.frequency.setValueAtTime(arpFreq, t);

                arpFilter.type = 'bandpass';
                arpFilter.frequency.setValueAtTime(2200, t);
                arpFilter.Q.value = 3.5;

                arpGain.gain.setValueAtTime(0.04, t);
                arpGain.gain.exponentialRampToValueAtTime(0.0005, t + 0.1);

                arpOsc.connect(arpFilter);
                arpFilter.connect(arpGain);
                arpGain.connect(this.ctx.destination);

                arpOsc.start(t);
                arpOsc.stop(t + 0.11);

                this.step++;
            }
        }

        const bgmEngine = new CyberAudioEngine();

        function toggleBGM() {
            const active = bgmEngine.toggle();
            const btn = document.getElementById('btn-toggle-bgm');
            const icon = document.getElementById('bgm-icon');
            const text = document.getElementById('bgm-status-text');

            if (active) {
                icon.className = 'fa-solid fa-volume-high text-cyan-400 animate-pulse';
                text.innerText = 'BGM: ON';
                btn.classList.add('border-cyan-400', 'bg-cyan-950/40');
            } else {
                icon.className = 'fa-solid fa-volume-xmark';
                text.innerText = 'BGM: OFF';
                btn.classList.remove('border-cyan-400', 'bg-cyan-950/40');
            }
        }

        // --- DATA & CONSTANTS ---
        const ZONES = [
            { id: 0, name: "中央スクエア", type: "City", danger: 1 },
            { id: 1, name: "軍事基地", type: "Military", danger: 3 },
            { id: 2, name: "密林エリア", type: "Forest", danger: 2 },
            { id: 3, name: "廃墟ビル街", type: "Ruins", danger: 2 },
            { id: 4, name: "研究シェルター", type: "Shelter", danger: 1 },
            { id: 5, name: "臨海工業地帯", type: "Factory", danger: 2 },
            { id: 6, name: "物流港湾", type: "Port", danger: 2 },
            { id: 7, name: "山岳観測所", type: "Mountain", danger: 1 },
            { id: 8, name: "地下プラント", type: "Subway", danger: 3 }
        ];

        const ITEM_DATABASE = [
            // Weapons
            { id: 'w1', name: 'サバイバルナイフ', type: 'weapon', atk: 8, rarity: 'Common', icon: 'fa-hand-back-fist' },
            { id: 'w2', name: '9mm ハンドガン', type: 'weapon', atk: 18, rarity: 'Common', icon: 'fa-gun' },
            { id: 'w3', name: 'ショットガン', type: 'weapon', atk: 35, rarity: 'Rare', icon: 'fa-gun' },
            { id: 'w4', name: 'アサルトライフル', type: 'weapon', atk: 45, rarity: 'Rare', icon: 'fa-gun' },
            { id: 'w5', name: 'スナイパーライフル', type: 'weapon', atk: 70, rarity: 'Epic', icon: 'fa-crosshairs' },
            
            // Armors
            { id: 'a1', name: '防弾チョッキ Lv1', type: 'armor', def: 15, rarity: 'Common', icon: 'fa-vest' },
            { id: 'a2', name: 'タクティカルベスト Lv2', type: 'armor', def: 30, rarity: 'Rare', icon: 'fa-vest' },
            { id: 'a3', name: '特殊部隊アーマー Lv3', type: 'armor', def: 45, rarity: 'Epic', icon: 'fa-shield-halved' },
            
            // Meds
            { id: 'm1', name: '応急包帯', type: 'med', heal: 25, rarity: 'Common', icon: 'fa-bandage' },
            { id: 'm2', name: '医療キット', type: 'med', heal: 55, rarity: 'Rare', icon: 'fa-kit-medical' },
            { id: 'm3', name: 'エナジードリンク', type: 'med', heal: 35, rarity: 'Common', icon: 'fa-bottle-water' }
        ];

        const PERKS = {
            assault: { name: 'アサルト', atkMod: 1.15, defMod: 1.0, hpMax: 100, initItem: 'w2' },
            scout: { name: 'スカウト', atkMod: 1.0, defMod: 1.0, hpMax: 100, dodgeBonus: 0.1, initItem: 'm1' },
            medic: { name: 'メディック', atkMod: 1.0, defMod: 1.0, hpMax: 100, healMod: 1.3, initItem: 'm2' },
            tank: { name: 'タフネス', atkMod: 1.0, defMod: 1.1, hpMax: 125, initItem: 'a1' }
        };

        const AI_NAMES = [
            "Shadow", "Viper", "Ghost", "Alpha", "Omega", "Reaper", "Blaze", "Raven", "Hunter", "Titan",
            "Spectre", "Cobra", "Phoenix", "Maverick", "Falcon", "Zero", "Nexus", "Storm", "Wolf", "Kestrel",
            "サイバー侍", "電脳浪人", "黒狐", "赤鬼", "疾風", "雷電", "影狼", "紫電", "迷彩兵", "鋼鉄男",
            "CyberPunk99", "AIBot_01", "NeonSniper", "VaporBlade", "GridWalker", "Aegis_X"
        ];

        // --- GAME STATE ---
        let gameState = {
            turn: 1,
            maxPlayers: 50,
            botDifficulty: 'normal',
            player: null,
            competitors: [],
            safeZoneIds: [0, 1, 2, 3, 4, 5, 6, 7, 8],
            ringTimer: 4,
            inCombatWith: null,
            isGuarding: false,
            isGameOver: false
        };

        // --- GAME INITIALIZATION ---
        function initGame() {
            // BGMがオフならゲームスタート時に自動起動を試みる
            if (!bgmEngine.isPlaying) {
                toggleBGM();
            }

            const nameInput = document.getElementById('player-name-input').value.trim() || "Operator";
            const perkKey = document.getElementById('player-perk-input').value;
            const diffKey = document.getElementById('game-difficulty-input').value;

            let totalCount = 50;
            if (diffKey === 'easy') totalCount = 10;
            if (diffKey === 'hard') totalCount = 99;

            gameState.maxPlayers = totalCount;
            gameState.botDifficulty = diffKey;
            const perkData = PERKS[perkKey];

            // Player Setup
            gameState.player = {
                name: nameInput,
                perk: perkKey,
                hp: perkData.hpMax,
                maxHp: perkData.hpMax,
                weapon: null,
                armor: null,
                inventory: [],
                zoneId: Math.floor(Math.random() * 9),
                kills: 0,
                isAlive: true
            };

            // Equip initial item
            if (perkData.initItem) {
                const initItemData = ITEM_DATABASE.find(i => i.id === perkData.initItem);
                if (initItemData) {
                    if (initItemData.type === 'weapon') gameState.player.weapon = initItemData;
                    if (initItemData.type === 'armor') gameState.player.armor = initItemData;
                    if (initItemData.type === 'med') gameState.player.inventory.push(initItemData);
                }
            }

            // Create BOT Competitors
            gameState.competitors = [];
            for (let i = 1; i < totalCount; i++) {
                const nameBase = AI_NAMES[Math.floor(Math.random() * AI_NAMES.length)];
                const randomWeapon = Math.random() < 0.5 ? ITEM_DATABASE[Math.floor(Math.random() * 4)] : null;
                const randomArmor = Math.random() < 0.4 ? ITEM_DATABASE[5] : null;

                const botMeds = [];
                if (Math.random() < 0.6) botMeds.push(ITEM_DATABASE[7]);

                gameState.competitors.push({
                    id: i,
                    name: `${nameBase}_${i}`,
                    hp: 100,
                    maxHp: 100,
                    weapon: randomWeapon,
                    armor: randomArmor,
                    inventory: botMeds,
                    zoneId: Math.floor(Math.random() * 9),
                    kills: 0,
                    aggro: Math.random(),
                    isAlive: true
                });
            }

            gameState.turn = 1;
            gameState.safeZoneIds = [0, 1, 2, 3, 4, 5, 6, 7, 8];
            gameState.ringTimer = 4;
            gameState.inCombatWith = null;
            gameState.isGuarding = false;
            gameState.isGameOver = false;

            // Clear UI elements
            document.getElementById('log-terminal').innerHTML = '';
            document.getElementById('kill-feed').innerHTML = '';

            document.getElementById('start-screen').classList.add('hidden');
            document.getElementById('end-screen').classList.add('hidden');

            addLog(`[作戦開始] プレイヤー「${nameInput}」降下完了。参加者数: 全${totalCount}名`, 'cyan');
            addLog(`初期降下位置: ${ZONES[gameState.player.zoneId].name}`, 'slate');

            updateUI();
        }

        // --- HELPER LOG & FEED FUNCTIONS ---
        function addLog(msg, color = 'slate') {
            const terminal = document.getElementById('log-terminal');
            const entry = document.createElement('div');
            let colorClass = 'text-slate-300';
            if (color === 'cyan') colorClass = 'text-cyan-400 font-semibold';
            if (color === 'red') colorClass = 'text-red-400 font-semibold';
            if (color === 'amber') colorClass = 'text-amber-400';
            if (color === 'emerald') colorClass = 'text-emerald-400';

            entry.className = colorClass;
            entry.innerHTML = `<span class="text-slate-600 mr-1">[T${gameState.turn}]</span> ${msg}`;
            terminal.appendChild(entry);
            terminal.scrollTop = terminal.scrollHeight;
        }

        function addKillFeed(msg) {
            const feed = document.getElementById('kill-feed');
            const entry = document.createElement('div');
            entry.className = 'text-slate-400 truncate border-b border-slate-900 pb-0.5';
            entry.innerHTML = `<span class="text-red-400"><i class="fa-solid fa-skull mr-1"></i></span>${msg}`;
            feed.insertBefore(entry, feed.firstChild);
        }

        // --- MAP CLICK HANDLER ---
        function clickCell(zoneId) {
            if (gameState.isGameOver) return;
            if (gameState.inCombatWith) {
                addLog('戦闘中は移動できません！攻撃・防御または逃走を選択してください。', 'red');
                return;
            }
            if (gameState.player.zoneId === zoneId) {
                addLog(`現在地 [${ZONES[zoneId].name}] に滞在中です。`, 'slate');
                return;
            }
            moveToZone(zoneId);
        }

        function moveToZone(targetZoneId) {
            gameState.player.zoneId = targetZoneId;
            addLog(`エリア移動: [${ZONES[targetZoneId].name}] へ移動しました。`, 'cyan');
            endTurn();
        }

        function openMoveModal() {
            if (gameState.inCombatWith) {
                addLog('戦闘中はエリア移動できません。', 'red');
                return;
            }
            const list = document.getElementById('move-list');
            list.innerHTML = '';
            ZONES.forEach(z => {
                const isCurrent = gameState.player.zoneId === z.id;
                const isSafe = gameState.safeZoneIds.includes(z.id);
                const btn = document.createElement('button');
                btn.className = `w-full text-left p-2.5 rounded-lg border text-xs flex justify-between items-center ${
                    isCurrent ? 'bg-slate-800 border-cyan-500/50 text-cyan-400' : 'bg-slate-950 hover:bg-slate-800 border-slate-800 text-slate-200'
                }`;
                btn.onclick = () => {
                    document.getElementById('move-modal').classList.add('hidden');
                    moveToZone(z.id);
                };
                btn.innerHTML = `
                    <span>${z.name} ${isCurrent ? '(現在地)' : ''}</span>
                    <span class="${isSafe ? 'text-emerald-400' : 'text-red-400'} font-bold">${isSafe ? '安全' : '汚染区域'}</span>
                `;
                list.appendChild(btn);
            });
            document.getElementById('move-modal').classList.remove('hidden');
        }

        // --- PLAYER ACTIONS ---
        function actionSearch() {
            if (gameState.inCombatWith) return;
            addLog(`[${ZONES[gameState.player.zoneId].name}] を探索中...`, 'slate');
            
            const rand = Math.random();
            if (rand < 0.6) {
                // Item found
                const item = ITEM_DATABASE[Math.floor(Math.random() * ITEM_DATABASE.length)];
                if (gameState.player.inventory.length < 6) {
                    gameState.player.inventory.push(item);
                    addLog(`物資を発見: [${item.name}] を獲得しました！`, 'emerald');
                } else {
                    addLog(`[${item.name}] を発見しましたが、インベントリが一杯です。`, 'amber');
                }
            } else {
                addLog('有用な物資は見つかりませんでした。', 'slate');
            }
            endTurn();
        }

        function actionRest() {
            if (gameState.inCombatWith) return;
            const healAmt = 20;
            gameState.player.hp = Math.min(gameState.player.maxHp, gameState.player.hp + healAmt);
            addLog(`物陰で待機して体力を回復 (+${healAmt} HP)。現在HP: ${gameState.player.hp}`, 'emerald');
            endTurn();
        }

        function actionScan() {
            if (gameState.inCombatWith) return;
            const inArea = gameState.competitors.filter(c => c.isAlive && c.zoneId === gameState.player.zoneId);
            addLog(`レーダースキャン完了: 現在地に敵プレイヤーが ${inArea.length} 名検知されました。`, 'amber');
            endTurn(false); // Scan cost half turn or info only
        }

        // --- COMBAT ACTIONS ---
        function triggerFx(type, text = '', isCrit = false) {
            const container = document.getElementById('tactical-container');
            
            // 画面揺れ
            container.classList.remove('shake-effect');
            void container.offsetWidth; // リフロー発生でアニメーション再起動
            container.classList.add('shake-effect');

            // 攻撃時のスラッシュ（斬撃/銃撃）視覚エフェクトを追加
            if (type === 'enemy-damage' || type === 'player-damage') {
                const slash = document.createElement('div');
                slash.className = isCrit ? 'slash-effect-crit' : 'slash-effect';
                container.appendChild(slash);
                setTimeout(() => slash.remove(), 400);
            }

            // 赤フラッシュ / 会心フラッシュ
            const flash = document.createElement('div');
            flash.className = isCrit ? 'crit-flash' : 'hit-flash';
            document.body.appendChild(flash);
            setTimeout(() => flash.remove(), 400);

            // ダメージ・効果テキストポップアップ
            if (text) {
                const pop = document.createElement('div');
                pop.className = `damage-pop-el text-2xl sm:text-3xl ${
                    isCrit ? 'text-amber-400 glow-yellow scale-125' : 
                    type === 'player-damage' ? 'text-red-500 glow-red' : 
                    type === 'enemy-damage' ? 'text-cyan-400 glow-cyan' : 'text-emerald-400'
                }`;
                pop.innerHTML = text;
                container.appendChild(pop);
                setTimeout(() => pop.remove(), 850);
            }
        }

        function startCombat(enemy) {
            gameState.inCombatWith = enemy;
            triggerFx('start', '⚡ ENCOUNTER! ⚡');
            addLog(`⚠️ 敵プレイヤー [${enemy.name}] と遭遇！戦闘開始！`, 'red');
            updateUI();
        }

        function combatAttack() {
            if (!gameState.inCombatWith) return;
            const enemy = gameState.inCombatWith;
            
            // Player Attack Calc
            const baseAtk = 10 + (gameState.player.weapon ? gameState.player.weapon.atk : 0);
            const perk = PERKS[gameState.player.perk];
            const isCrit = Math.random() < 0.25; // 25%の確率でクリティカルヒット
            const critMod = isCrit ? 1.6 : 1.0;
            const finalAtk = Math.floor(baseAtk * perk.atkMod * (0.9 + Math.random() * 0.2) * critMod);
            
            // Enemy Armor Calc
            const armorDef = enemy.armor ? enemy.armor.def : 0;
            const damage = Math.max(5, Math.floor(finalAtk * (1 - armorDef / 100)));

            enemy.hp -= damage;
            const enemyHpAfter = Math.max(0, enemy.hp);
            
            // 演出実行
            triggerFx('enemy-damage', `${isCrit ? '💥 CRITICAL! ' : ''}-${damage}`, isCrit);

            if (isCrit) {
                addLog(`⚡ クリティカル！！ ${enemy.name} に ${damage} ダメージ！ (敵HP: ${enemyHpAfter}/${enemy.maxHp} | 自分HP: ${gameState.player.hp}/${gameState.player.maxHp})`, 'amber');
            } else {
                addLog(`⚔️ ${enemy.name} に攻撃！ ${damage} ダメージ！ (敵HP: ${enemyHpAfter}/${enemy.maxHp} | 自分HP: ${gameState.player.hp}/${gameState.player.maxHp})`, 'cyan');
            }

            if (enemy.hp <= 0) {
                enemy.isAlive = false;
                gameState.player.kills++;
                triggerFx('heal', '☠️ K.O.!', true);
                addLog(`💥 [${enemy.name}] を撃破しました！ (自分HP: ${gameState.player.hp}/${gameState.player.maxHp})`, 'emerald');
                addKillFeed(`${gameState.player.name} が ${enemy.name} を倒した`);
                
                // Loot Item
                if (enemy.weapon || enemy.armor) {
                    const loot = enemy.weapon || enemy.armor;
                    addLog(`戦利品: [${loot.name}] を入手しました。`, 'amber');
                    if (loot.type === 'weapon' && (!gameState.player.weapon || loot.atk > gameState.player.weapon.atk)) {
                        gameState.player.weapon = loot;
                    } else if (loot.type === 'armor' && (!gameState.player.armor || loot.def > gameState.player.armor.def)) {
                        gameState.player.armor = loot;
                    }
                }
                gameState.inCombatWith = null;
                endTurn();
                return;
            }

            // Enemy Counter-attack
            enemyCounter();
        }

        function combatDefend() {
            if (!gameState.inCombatWith) return;
            gameState.isGuarding = true;
            triggerFx('guard', '🛡️ BLOCK');
            addLog(`🛡️ 防御姿勢をとりました。受けるダメージが半減します。 (自分HP: ${gameState.player.hp}/${gameState.player.maxHp})`, 'slate');
            enemyCounter();
            gameState.isGuarding = false;
        }

        function combatUseItem() {
            if (!gameState.inCombatWith) return;
            const medIndex = gameState.player.inventory.findIndex(i => i.type === 'med');
            if (medIndex !== -1) {
                const med = gameState.player.inventory.splice(medIndex, 1)[0];
                gameState.player.hp = Math.min(gameState.player.maxHp, gameState.player.hp + med.heal);
                triggerFx('heal', `+${med.heal} HP`);
                addLog(`💊 [${med.name}] を使用！ HP+${med.heal} 回復 (自分HP: ${gameState.player.hp}/${gameState.player.maxHp})`, 'emerald');
                enemyCounter();
            } else {
                addLog(`回復アイテムを所持していません！`, 'red');
            }
        }

        function combatFlee() {
            if (!gameState.inCombatWith) return;
            if (Math.random() < 0.6) {
                triggerFx('flee', '💨 ESCAPED!');
                addLog(`💨 戦闘からの離脱に成功しました！ (自分HP: ${gameState.player.hp}/${gameState.player.maxHp})`, 'amber');
                gameState.inCombatWith = null;
                endTurn();
            } else {
                addLog(`❌ 逃走失敗！敵に背後を見せてしまった。 (自分HP: ${gameState.player.hp}/${gameState.player.maxHp})`, 'red');
                enemyCounter();
            }
        }

        function enemyCounter() {
            const enemy = gameState.inCombatWith;
            if (!enemy || !enemy.isAlive) return;

            const enemyAtk = 10 + (enemy.weapon ? enemy.weapon.atk : 0);
            const playerDef = (gameState.player.armor ? gameState.player.armor.def : 0) * (PERKS[gameState.player.perk].defMod || 1);
            
            let damage = Math.max(3, Math.floor(enemyAtk * (1 - playerDef / 100)));
            if (gameState.isGuarding) damage = Math.floor(damage * 0.5);

            gameState.player.hp = Math.max(0, gameState.player.hp - damage);
            const enemyHpCurrent = Math.max(0, enemy.hp);
            
            // 被ダメージ演出
            setTimeout(() => {
                triggerFx('player-damage', `-${damage}`);
                addLog(`💥 [${enemy.name}] の反撃！ ${damage} ダメージ！ (自分HP: ${gameState.player.hp}/${gameState.player.maxHp} | 敵HP: ${enemyHpCurrent}/${enemy.maxHp})`, 'red');

                if (gameState.player.hp <= 0) {
                    gameOver(false);
                }
                updateUI();
            }, 250);
        }

        function combatAttack() {
            if (!gameState.inCombatWith) return;
            const enemy = gameState.inCombatWith;
            
            // Player Attack Calc
            const baseAtk = 10 + (gameState.player.weapon ? gameState.player.weapon.atk : 0);
            const perk = PERKS[gameState.player.perk];
            const finalAtk = Math.floor(baseAtk * perk.atkMod * (0.9 + Math.random() * 0.2));
            
            // Enemy Armor Calc
            const armorDef = enemy.armor ? enemy.armor.def : 0;
            const damage = Math.max(5, Math.floor(finalAtk * (1 - armorDef / 100)));

            enemy.hp -= damage;
            addLog(`⚔️ ${enemy.name} に攻撃！ ${damage} のダメージを与えた！ (敵残りHP: ${Math.max(0, enemy.hp)})`, 'cyan');

            if (enemy.hp <= 0) {
                enemy.isAlive = false;
                gameState.player.kills++;
                addLog(`💥 [${enemy.name}] を撃破しました！`, 'emerald');
                addKillFeed(`${gameState.player.name} が ${enemy.name} を倒した`);
                
                // Loot Item
                if (enemy.weapon || enemy.armor) {
                    const loot = enemy.weapon || enemy.armor;
                    addLog(`戦利品: [${loot.name}] を入手しました。`, 'amber');
                    if (loot.type === 'weapon' && (!gameState.player.weapon || loot.atk > gameState.player.weapon.atk)) {
                        gameState.player.weapon = loot;
                    } else if (loot.type === 'armor' && (!gameState.player.armor || loot.def > gameState.player.armor.def)) {
                        gameState.player.armor = loot;
                    }
                }
                gameState.inCombatWith = null;
                endTurn();
                return;
            }

            // Enemy Counter-attack
            enemyCounter();
        }

        function combatDefend() {
            if (!gameState.inCombatWith) return;
            gameState.isGuarding = true;
            addLog(`🛡️ 防御姿勢をとりました。受けるダメージが半減します。`, 'slate');
            enemyCounter();
            gameState.isGuarding = false;
        }

        function combatUseItem() {
            if (!gameState.inCombatWith) return;
            const medIndex = gameState.player.inventory.findIndex(i => i.type === 'med');
            if (medIndex !== -1) {
                const med = gameState.player.inventory.splice(medIndex, 1)[0];
                gameState.player.hp = Math.min(gameState.player.maxHp, gameState.player.hp + med.heal);
                addLog(`💊 [${med.name}] を使用して HPが ${med.heal} 回復！ (現在HP: ${gameState.player.hp})`, 'emerald');
                enemyCounter();
            } else {
                addLog(`回復アイテムを所持していません！`, 'red');
            }
        }

        function combatFlee() {
            if (!gameState.inCombatWith) return;
            if (Math.random() < 0.6) {
                addLog(`💨 戦闘からの離脱に成功しました！`, 'amber');
                gameState.inCombatWith = null;
                endTurn();
            } else {
                addLog(`❌ 逃走失敗！敵に背後を見せてしまった。`, 'red');
                enemyCounter();
            }
        }

        function enemyCounter() {
            const enemy = gameState.inCombatWith;
            if (!enemy || !enemy.isAlive) return;

            const enemyAtk = 10 + (enemy.weapon ? enemy.weapon.atk : 0);
            const playerDef = (gameState.player.armor ? gameState.player.armor.def : 0) * (PERKS[gameState.player.perk].defMod || 1);
            
            let damage = Math.max(3, Math.floor(enemyAtk * (1 - playerDef / 100)));
            if (gameState.isGuarding) damage = Math.floor(damage * 0.5);

            gameState.player.hp -= damage;
            addLog(`💥 [${enemy.name}] の反撃！ ${damage} のダメージを受けた！`, 'red');

            if (gameState.player.hp <= 0) {
                gameState.player.hp = 0;
                gameOver(false);
            }
            updateUI();
        }

        // --- TURN & BOT SIMULATION ---
        function endTurn(advanceTurn = true) {
            if (gameState.isGameOver) return;

            if (advanceTurn) {
                gameState.turn++;

                // Check Zone Ring shrinking
                gameState.ringTimer--;
                if (gameState.ringTimer <= 0) {
                    shrinkSafeZone();
                    gameState.ringTimer = 4;
                }

                // Zone Radiation Damage
                if (!gameState.safeZoneIds.includes(gameState.player.zoneId)) {
                    const radDamage = 15;
                    gameState.player.hp -= radDamage;
                    addLog(`☢️ 安全地帯外の毒ガスにより ${radDamage} ダメージを受けた！`, 'red');
                    if (gameState.player.hp <= 0) {
                        gameState.player.hp = 0;
                        gameOver(false);
                        return;
                    }
                }

                // Simulate BOT Activity
                simulateBots();

                // Check for Encounter if not in combat
                if (!gameState.inCombatWith) {
                    const localEnemies = gameState.competitors.filter(c => c.isAlive && c.zoneId === gameState.player.zoneId);
                    if (localEnemies.length > 0 && Math.random() < 0.75) {
                        const enemy = localEnemies[Math.floor(Math.random() * localEnemies.length)];
                        startCombat(enemy);
                    }
                }
            }

            // Check Win condition
            const aliveBots = gameState.competitors.filter(c => c.isAlive).length;
            if (aliveBots === 0 && gameState.player.hp > 0) {
                gameOver(true);
                return;
            }

            updateUI();
        }

        function shrinkSafeZone() {
            if (gameState.safeZoneIds.length <= 1) return;
            const removed = gameState.safeZoneIds.pop();
            addLog(`⚠️ 安全地帯が縮小しました！ [${ZONES[removed].name}] が危険区域化！`, 'amber');
        }

        function simulateBots() {
            const aliveBots = gameState.competitors.filter(c => c.isAlive);
            
            // Random Bot Eliminations & Movements
            aliveBots.forEach(bot => {
                // Move if unsafe
                if (!gameState.safeZoneIds.includes(bot.zoneId) && Math.random() < 0.8) {
                    bot.zoneId = gameState.safeZoneIds[Math.floor(Math.random() * gameState.safeZoneIds.length)];
                } else if (Math.random() < 0.3) {
                    bot.zoneId = Math.floor(Math.random() * 9);
                }

                // Bot Damage outside ring
                if (!gameState.safeZoneIds.includes(bot.zoneId)) {
                    bot.hp -= 20;
                    if (bot.hp <= 0) {
                        bot.isAlive = false;
                        addKillFeed(`${bot.name} が汚染エリアで脱落した`);
                    }
                }
            });

            // Random BOT vs BOT fights
            for (let i = 0; i < aliveBots.length; i++) {
                if (!aliveBots[i].isAlive) continue;
                for (let j = i + 1; j < aliveBots.length; j++) {
                    if (!aliveBots[j].isAlive) continue;

                    if (aliveBots[i].zoneId === aliveBots[j].zoneId && Math.random() < 0.25) {
                        // Duel
                        const winner = Math.random() < 0.5 ? aliveBots[i] : aliveBots[j];
                        const loser = winner === aliveBots[i] ? aliveBots[j] : aliveBots[i];
                        loser.isAlive = false;
                        winner.kills++;
                        addKillFeed(`${winner.name} が ${loser.name} を倒した`);
                    }
                }
            }
        }

        function useInventoryItem(index) {
            if (gameState.isGameOver) return;
            const item = gameState.player.inventory[index];
            if (!item) return;

            if (item.type === 'weapon') {
                const oldWeapon = gameState.player.weapon;
                gameState.player.weapon = item;
                gameState.player.inventory.splice(index, 1);
                if (oldWeapon) gameState.player.inventory.push(oldWeapon);
                addLog(`装備変更: [${item.name}] を装備しました。`, 'cyan');
            } else if (item.type === 'armor') {
                const oldArmor = gameState.player.armor;
                gameState.player.armor = item;
                gameState.player.inventory.splice(index, 1);
                if (oldArmor) gameState.player.inventory.push(oldArmor);
                addLog(`防具変更: [${item.name}] を装着しました。`, 'cyan');
            } else if (item.type === 'med') {
                gameState.player.hp = Math.min(gameState.player.maxHp, gameState.player.hp + item.heal);
                gameState.player.inventory.splice(index, 1);
                addLog(`回復使用: [${item.name}] で HPが ${item.heal} 回復！`, 'emerald');
            }
            updateUI();
        }

        function gameOver(isWin) {
            gameState.isGameOver = true;
            const aliveBots = gameState.competitors.filter(c => c.isAlive).length;
            const rank = isWin ? 1 : aliveBots + 1;

            document.getElementById('end-title').innerText = isWin ? "VICTORY ROYALE!" : "DEFEAT...";
            document.getElementById('end-title').className = isWin ? "text-2xl font-extrabold tracking-wider text-amber-400 glow-cyan" : "text-2xl font-extrabold tracking-wider text-red-500 glow-red";
            document.getElementById('end-subtitle').innerText = isWin ? "最後の1人として生き残りました！ドン勝つ！" : "戦場にて倒れました。";
            document.getElementById('end-rank').innerText = `#${rank} / ${gameState.maxPlayers}`;
            document.getElementById('end-kills').innerText = gameState.player.kills;
            document.getElementById('end-turns').innerText = gameState.turn;
            document.getElementById('end-perk').innerText = PERKS[gameState.player.perk].name;

            document.getElementById('end-screen').classList.remove('hidden');
        }

        // --- UI RENDER & UPDATE ---
        function updateUI() {
            if (!gameState.player) return;

            const aliveCount = gameState.competitors.filter(c => c.isAlive).length + (gameState.player.hp > 0 ? 1 : 0);
            
            // Header Stats
            document.getElementById('header-alive').innerText = aliveCount;
            document.getElementById('header-total').innerText = gameState.maxPlayers;
            document.getElementById('header-kills').innerText = gameState.player.kills;
            document.getElementById('header-turn').innerText = gameState.turn;

            // Player Card
            document.getElementById('player-name-display').innerText = gameState.player.name;
            document.getElementById('player-perk-tag').innerText = PERKS[gameState.player.perk].name;
            document.getElementById('hp-text').innerText = `${gameState.player.hp} / ${gameState.player.maxHp}`;
            
            const hpPercent = Math.max(0, (gameState.player.hp / gameState.player.maxHp) * 100);
            document.getElementById('hp-bar').style.width = `${hpPercent}%`;

            const armorDef = gameState.player.armor ? gameState.player.armor.def : 0;
            const totalAtk = 10 + (gameState.player.weapon ? gameState.player.weapon.atk : 0);
            document.getElementById('status-armor').innerText = `${armorDef}%`;
            document.getElementById('status-atk').innerText = totalAtk;

            // Equipment & Inventory
            document.getElementById('eq-weapon').innerText = gameState.player.weapon ? gameState.player.weapon.name : "素手 (ATK +0)";
            document.getElementById('eq-armor').innerText = gameState.player.armor ? gameState.player.armor.name : "なし (DEF 0%)";
            document.getElementById('inventory-capacity').innerText = `${gameState.player.inventory.length}/6 Slots`;

            const invList = document.getElementById('inventory-list');
            invList.innerHTML = '';
            gameState.player.inventory.forEach((item, index) => {
                const itemDiv = document.createElement('div');
                itemDiv.className = "bg-slate-950 p-2 rounded-lg border border-slate-800 flex justify-between items-center text-xs hover:border-slate-700 transition";
                itemDiv.innerHTML = `
                    <div class="flex items-center gap-2 overflow-hidden">
                        <i class="fa-solid ${item.icon} text-cyan-400"></i>
                        <span class="truncate text-slate-300 font-medium">${item.name}</span>
                    </div>
                    <button onclick="useInventoryItem(${index})" class="px-2 py-1 bg-slate-800 hover:bg-cyan-900/60 text-cyan-300 rounded border border-slate-700 text-[10px] shrink-0">
                        ${item.type === 'med' ? '使用' : '装備'}
                    </button>
                `;
                invList.appendChild(itemDiv);
            });

            // Map Render
            ZONES.forEach(z => {
                const cell = document.getElementById(`cell-${z.id}`);
                const isPlayerHere = gameState.player.zoneId === z.id;
                const isSafe = gameState.safeZoneIds.includes(z.id);
                const enemiesInZone = gameState.competitors.filter(c => c.isAlive && c.zoneId === z.id).length;

                cell.className = `bg-slate-950 border rounded-xl p-2 flex flex-col justify-between cursor-pointer transition relative overflow-hidden group ${
                    isPlayerHere ? 'border-cyan-400 ring-1 ring-cyan-400/50 bg-slate-900' : isSafe ? 'border-slate-800 hover:border-slate-700' : 'border-red-900/80 bg-red-950/20'
                }`;

                cell.innerHTML = `
                    <div class="flex justify-between items-start text-xs">
                        <span class="font-bold ${isPlayerHere ? 'text-cyan-400' : 'text-slate-300'}">${z.name}</span>
                        <span class="text-[9px] px-1.5 py-0.5 rounded font-bold ${isSafe ? 'bg-emerald-950 text-emerald-400 border border-emerald-800' : 'bg-red-950 text-red-400 border border-red-800'}">
                            ${isSafe ? 'SAFE' : 'DANGER'}
                        </span>
                    </div>
                    <div class="flex justify-between items-end mt-4">
                        <div class="text-[11px] text-slate-500">
                            ${enemiesInZone > 0 ? `<span class="text-amber-400 font-bold"><i class="fa-solid fa-user-ninja mr-1"></i>${enemiesInZone}</span>` : '<span class="text-slate-600">気配なし</span>'}
                        </div>
                        ${isPlayerHere ? '<span class="text-xs text-cyan-400 font-extrabold glow-cyan"><i class="fa-solid fa-location-dot mr-1"></i>現在地</span>' : ''}
                    </div>
                `;
            });

            // Controls Toggle (Normal vs Combat)
            const normalControls = document.getElementById('normal-controls');
            const combatControls = document.getElementById('combat-controls');

            if (gameState.inCombatWith) {
                normalControls.classList.add('hidden');
                combatControls.classList.remove('hidden');
                combatControls.classList.add('grid');
            } else {
                normalControls.classList.remove('hidden');
                combatControls.classList.add('hidden');
                combatControls.classList.remove('grid');
            }

            document.getElementById('ring-status').innerText = `安全地帯収縮まで: ${gameState.ringTimer}ターン`;
        }

        // Event Listener Bindings
        window.onload = function() {
            document.getElementById('btn-toggle-bgm').onclick = toggleBGM;
            document.getElementById('btn-start-game').onclick = initGame;
            document.getElementById('btn-return-lobby').onclick = () => {
                document.getElementById('end-screen').classList.add('hidden');
                document.getElementById('start-screen').classList.remove('hidden');
            };

            document.getElementById('btn-action-search').onclick = actionSearch;
            document.getElementById('btn-action-rest').onclick = actionRest;
            document.getElementById('btn-action-scan').onclick = actionScan;
            document.getElementById('btn-action-move').onclick = openMoveModal;

            // Combat Actions
            document.getElementById('btn-combat-attack').onclick = combatAttack;
            document.getElementById('btn-combat-defend').onclick = combatDefend;
            document.getElementById('btn-combat-item').onclick = combatUseItem;
            document.getElementById('btn-combat-flee').onclick = combatFlee;

            document.getElementById('btn-close-move').onclick = () => {
                document.getElementById('move-modal').classList.add('hidden');
            };
        };
    </script>
</body>
</html>