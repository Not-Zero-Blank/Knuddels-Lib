
# Knuddels-Lib
Made by BLANK#3020

Die Implementation ist sehr schlecht wurde vor 3 Jahren gemacht und hab sie schnell auf Heute (17.02.2023) geupdated ggf sollten die Payloads in Payload.cs Angepasst werden in Zukunft.

Hat keine Websocket Implementation geschweige denn Proxy Implementation oder hwid spoof.

Hat auch keine Exploits wie zb Channel Force Disconnect / Force Channel Mute / Force Ratelimit.

Ich hab eine Private Lib womit ich auch meinen eigenen Knuddels Client & Bots gemacht habe. Die hat weitaus mehr als diese schlechte Lib. Access nur auf Anfrage.

# Usage
```
    var client = new Client("usernick", "password");
    client.Login();
    client.Start(); // Die 2 events (NewVisitor, NewPrivateMessage) müssen gebindet werden bevor diese Funktion aufgerufen wird sonst werden die Loops dafür nicht erstellt
```
# Features

Allgemein
 - Passwort Ändern
 - Nutzer Melden
 - Account geht nicht in den Abwesend Status
 - Der Account geht nicht Offline
 - Profil Besucher Auflisten 
 - Detaillierte User Information
 - Album Fotos von Usern auflisten

Channel Funktionen

 - Alle Verfügbaren Channels auflisten
 - User von Channels Auflisten
 - Channel Nachrichten senden (dazu muss zuerst dem Channel beigetreten werden, es reicht schon einfach "GetUserIDsFromChannel" aufzurufen mit der Channel ID)

Privater Chat
 - Private Nachrichten Lesen/Empfangen
 - Nur Bilder aus den Chats auflisten
 - Nachrichten als gelesen Makieren
 - Chats auflisten

