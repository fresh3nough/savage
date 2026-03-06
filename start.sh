#!/usr/bin/env bash
# start.sh - Master startup script for the Savage Inventory Management System.
#
# This script will:
#   1. Install nvm (if missing) and use it to install/activate Node 20.
#   2. Verify that required tools (node, npm, dotnet, mongod) are installed.
#   3. Ensure MongoDB is running.
#   4. Restore/install backend .NET dependencies.
#   5. Install frontend npm dependencies.
#   6. Start the ASP.NET Core backend on port 5000.
#   7. Start the React dev server on port 3000.
#   8. When you press Ctrl+C, both processes are stopped cleanly.

set -o pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
BACK_DIR="$SCRIPT_DIR/back/Savage.Api"
FRONT_DIR="$SCRIPT_DIR/front"

# ── Color helpers ──
RED='\033[0;31m'
GREEN='\033[0;32m'
CYAN='\033[0;36m'
NC='\033[0m'

info()  { echo -e "${CYAN}[INFO]${NC}  $1"; }
ok()    { echo -e "${GREEN}[OK]${NC}    $1"; }
fail()  { echo -e "${RED}[ERROR]${NC} $1"; exit 1; }

# ── 1. Install / load nvm and Node 20 ──
export NVM_DIR="${NVM_DIR:-$HOME/.nvm}"

if [ ! -d "$NVM_DIR" ]; then
    info "nvm not found. Installing nvm..."
    curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.40.1/install.sh | bash \
        || fail "nvm installation failed."
fi

# Load nvm into the current shell
[ -s "$NVM_DIR/nvm.sh" ] && . "$NVM_DIR/nvm.sh"
command -v nvm >/dev/null 2>&1 || fail "nvm failed to load. Check your ~/.nvm installation."
ok "nvm $(nvm --version) loaded."

# Install and use Node 20
if ! nvm ls 20 >/dev/null 2>&1; then
    info "Installing Node 20 via nvm..."
    nvm install 20 || fail "nvm install 20 failed."
fi
nvm use 20 || fail "nvm use 20 failed."
ok "node $(node --version)  |  npm $(npm --version) (managed by nvm)"

# ── 2. Check remaining prerequisites ──
info "Checking prerequisites..."
command -v dotnet >/dev/null 2>&1 || fail ".NET SDK is not installed. Run: sudo apt-get install -y dotnet-sdk-8.0"
ok "dotnet $(dotnet --version)"

# ── 3. Ensure MongoDB is running ──
info "Checking MongoDB..."
if command -v mongod >/dev/null 2>&1; then
    if ! pgrep -x mongod >/dev/null 2>&1; then
        info "MongoDB is installed but not running. Attempting to start..."
        sudo systemctl start mongod 2>/dev/null || sudo mongod --fork --logpath /tmp/mongod.log 2>/dev/null
        sleep 2
        pgrep -x mongod >/dev/null 2>&1 || fail "Could not start MongoDB. Start it manually: sudo systemctl start mongod"
    fi
    ok "MongoDB is running."
else
    fail "MongoDB is not installed. See README.md for install instructions."
fi

# ── 4. Backend: restore and build ──
info "Restoring backend .NET dependencies..."
dotnet restore "$BACK_DIR/Savage.Api.csproj" --verbosity quiet || fail "dotnet restore failed."
ok "Backend dependencies restored."

info "Building backend..."
dotnet build "$BACK_DIR/Savage.Api.csproj" --verbosity quiet --no-restore || fail "dotnet build failed."
ok "Backend build succeeded."

# ── 5. Frontend: npm install ──
info "Installing frontend npm dependencies..."
npm install --prefix "$FRONT_DIR" --silent || fail "npm install failed in front/."
ok "Frontend dependencies installed."

# ── 6 & 7. Launch in separate tmux panes ──
command -v tmux >/dev/null 2>&1 || fail "tmux is not installed. Run: sudo apt-get install -y tmux"

SESSION="savage"

# Kill any existing session with the same name.
tmux kill-session -t "$SESSION" 2>/dev/null

# Resolve the nvm-managed node/npm paths so tmux inherits them.
NODE_BIN="$(dirname "$(command -v node)")"

# Create a new detached session running the backend.
tmux new-session -d -s "$SESSION" -n "backend" \
    "echo -e '${CYAN}[BACKEND]${NC} Starting on http://localhost:5000 ...'; \
     dotnet run --project $BACK_DIR/Savage.Api.csproj --urls http://localhost:5000 --no-build; \
     echo -e '${RED}[BACKEND] Process exited. Press enter to close.${NC}'; read"

# Create a second window running the frontend, with nvm node on PATH.
tmux new-window -t "$SESSION" -n "frontend" \
    "export PATH=\"$NODE_BIN:\$PATH\"; \
     echo -e '${CYAN}[FRONTEND]${NC} Starting on http://localhost:3000 ...'; \
     BROWSER=none npm start --prefix $FRONT_DIR; \
     echo -e '${RED}[FRONTEND] Process exited. Press enter to close.${NC}'; read"

echo ""
echo -e "${GREEN}=============================================${NC}"
echo -e "${GREEN}  Savage is running in tmux session: $SESSION${NC}"
echo -e "${GREEN}  Frontend:  http://localhost:3000${NC}"
echo -e "${GREEN}  Backend:   http://localhost:5000${NC}"
echo -e "${GREEN}  Swagger:   http://localhost:5000/swagger${NC}"
echo -e "${GREEN}---------------------------------------------${NC}"
echo -e "${GREEN}  tmux attach -t $SESSION    (attach)${NC}"
echo -e "${GREEN}  Ctrl+B then N              (switch window)${NC}"
echo -e "${GREEN}  tmux kill-session -t $SESSION (stop all)${NC}"
echo -e "${GREEN}=============================================${NC}"
echo ""

# Attach to the session so the user lands in it.
tmux attach -t "$SESSION"
