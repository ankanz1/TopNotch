#!bash
# Setup .NET 8 SDK for TopNotch project

# Create temporary directory for installation
TEMP_DIR=$(mktemp -d)
cd $TEMP_DIR

# Download .NET 8 SDK installer
wget -q https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh

# Install to home directory (will be in ~/.dotnet)
./dotnet-install.sh --channel 8.0 --install-dir $HOME/.dotnet --verbose

# Create symlink in /usr/local for easier access
if [ -d "$HOME/.dotnet" ]; then
    ln -sf $HOME/.dotnet/dotnet /usr/local/bin/dotnet || true
fi

# Add to PATH if not already present
if ! grep -q "\.dotnet" ~/.bashrc; then
    echo "export PATH=\$HOME/.dotnet:\$PATH" >> ~/.bashrc
fi

cd /
rm -rf $TEMP_DIR

echo " dotnet SDK installed. Please restart shell or run: export PATH=\$HOME/.dotnet:\$PATH"
export PATH=$HOME/.dotnet:$PATH
dotnet --info