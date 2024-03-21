import spotipy
from spotipy.oauth2 import SpotifyOAuth
import webbrowser
from flask import Flask, request
import threading
import keyboard
from tkinter import *
from tkinter import simpledialog
import json
import os

# Define paths for the token cache file
TOKEN_INFO_FILE = "token_info.json"

# Define paths for the config file
CONFIG_FILE = "config.json"

# Replace these with your Spotify app credentials
CLIENT_ID = '1747b3ed9d0740c9a6ff29f1ca6997b8'
CLIENT_SECRET = 'c5ca7a8a2751480a86d63737f28e4c99'

# Load client credentials from config.json
def load_credentials():
    if os.path.exists(CONFIG_FILE):
        with open(CONFIG_FILE, 'r') as f:
            data = json.load(f)
            if data['CLIENT_ID'] and data['CLIENT_SECRET']:
                return data['CLIENT_ID'], data['CLIENT_SECRET']
    return CLIENT_ID, CLIENT_SECRET

CLIENT_ID, CLIENT_SECRET = load_credentials()
REDIRECT_URI = 'http://localhost:5000/callback'
SCOPE = 'user-library-modify user-read-currently-playing'

# Flask app for handling Spotify authorization
app = Flask(__name__)

# Global Spotify client, will be initialized after authorization
sp = None

def save_token_info(token_info):
    """Save the token info to a file."""
    with open(TOKEN_INFO_FILE, 'w') as file:
        json.dump(token_info, file)

def load_token_info():
    """Load the token info from a file if it exists."""
    if os.path.exists(TOKEN_INFO_FILE):
        with open(TOKEN_INFO_FILE, 'r') as file:
            token_info = json.load(file)
            return token_info
    return None

def refresh_token_if_expired(token_info):
    """Refresh the token if it is expired."""
    global sp
    if sp is None or (token_info and spotipy.oauth2.is_token_expired(token_info)):
        auth_manager = SpotifyOAuth(client_id=CLIENT_ID, client_secret=CLIENT_SECRET,
                                    redirect_uri=REDIRECT_URI, scope=SCOPE, cache_path=TOKEN_INFO_FILE)
        if token_info:
            token_info = auth_manager.refresh_access_token(token_info['refresh_token'])
        save_token_info(token_info)
    if token_info:
        sp = spotipy.Spotify(auth=token_info['access_token'])

@app.route('/')
def authorize():
    auth_manager = SpotifyOAuth(client_id=CLIENT_ID, client_secret=CLIENT_SECRET,
                                redirect_uri=REDIRECT_URI, scope=SCOPE, cache_path=TOKEN_INFO_FILE)
    auth_url = auth_manager.get_authorize_url()
    return f'<h2>Spotify Authorization</h2><p>Please <a href="{auth_url}">click here</a> to authorize the application.</p>'

@app.route('/callback')
def callback():
    global sp
    auth_manager = SpotifyOAuth(client_id=CLIENT_ID, client_secret=CLIENT_SECRET,
                                redirect_uri=REDIRECT_URI, scope=SCOPE, cache_path=TOKEN_INFO_FILE)
    code = request.args.get('code')
    token_info = auth_manager.get_access_token(code)
    save_token_info(token_info)
    sp = spotipy.Spotify(auth=token_info['access_token'])
    return '<h2>Authorization Successful!</h2><p>You can now close this window.</p>'


def add_current_track_to_favorites():
    global sp
    if sp:
        current_track = sp.current_user_playing_track()
        if current_track:
            track_id = current_track['item']['id']
            sp.current_user_saved_tracks_add(tracks=[track_id])
            update_status(f"Track {current_track['item']['name']} by {current_track['item']['artists'][0]['name']} added to favorites.")
        else:
            update_status("No track is currently playing.")
    else:
        update_status("Spotify client not initialized.")

def run_flask_app():
    app.run(port=5000)

def set_hotkey():
    hotkey = simpledialog.askstring("Input", "Enter your desired hotkey (e.g., ctrl+shift+tab):",
                                    parent=tk_root)
    if hotkey:
        keyboard.add_hotkey(hotkey, add_current_track_to_favorites)
        update_hotkey_tip(hotkey)
        print(f"Hotkey set to {hotkey}. Press it to favorite the current track on Spotify.")

def update_hotkey_tip(hotkey):
    hotkey_tip_label.config(text=f"Hotkey set: {hotkey}")

def update_status(message):
    status_label.config(text=message)

if __name__ == '__main__':
    token_info = load_token_info()
    refresh_token_if_expired(token_info)

    if sp is None:
        threading.Thread(target=run_flask_app, daemon=True).start()
        webbrowser.open('http://localhost:5000')
    else:
        print("Already authorized. No need to open the browser.")

    tk_root = Tk()
    tk_root.geometry('350x200')  # Adjusted window size for the additional status label
    tk_root.title('Spotify Favorites Hotkey Setter')

    Label(tk_root, text="Press the button below to set your Spotify Favorites Hotkey.").pack(pady=20)
    Button(tk_root, text="Set Hotkey", command=set_hotkey).pack(pady=10)
    hotkey_tip_label = Label(tk_root, text="No hotkey set.")
    hotkey_tip_label.pack(pady=10)
    status_label = Label(tk_root, text="Status will appear here.", wraplength=300)
    status_label.pack(pady=10)

    keyboard.add_hotkey("ctrl+shift+tab", add_current_track_to_favorites)
    update_hotkey_tip("ctrl+shift+tab")

    tk_root.mainloop()
