﻿using SimulationFramework.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Uno.Actions;
using Uno.Packets;

namespace Uno.Client.Gameplay;
internal class GameplayScene : GameScene
{
    // this player
    public string PlayerName { get; set; }
    public PlayerHand PlayerHand { get; set; }

    // all other players
    public Dictionary<string, PlayerHand> OtherPlayers { get; set; } = new();

    // stack where players play cards onto
    public InteractableCardStack PlayStack { get; set; }
    
    // stack where players draw cards from
    public InteractableCardStack DrawStack { get; set; }
    public Camera Camera;


    public GameplayScene(string playerName, List<string> allPlayers)
    {
        PlayerName = playerName;
        Camera = new(4);

        PlayerHand = new(Enumerable.Empty<InteractableCard>()) { Position = new(0, 2), SelectionEnabled = true };
        PlayStack = new() { Position = new(0, 0) };
        DrawStack = new() { Position = new(-1, 0) };

        var rng = new Random(53);
        for (int i = 0; i < 100; i++)
        {
            DrawStack.Cards.Push(new InteractableCard(CardFace.Random(rng)) { IsFaceDown = true });
        }

        foreach (var player in allPlayers)
        {
            if (player == playerName)
                continue;

            OtherPlayers.Add(player, new(Enumerable.Empty<InteractableCard>()) {  Position = Vector2.UnitY * -8, Rotation = Angle.ToRadians(180), Scale = .5f });
        }
    }

    public override void Render(ICanvas canvas)
    {
        Camera.ApplyTo(canvas);

        DrawStack.Render(canvas);
        PlayStack.Render(canvas); 
        PlayerHand.Render(canvas);

        foreach (var (_, hand) in OtherPlayers)
        {
            hand.Render(canvas);
        }

        base.Render(canvas);
    }

    public override void Update()
    {
        foreach (var packet in Client.Receive<PlayerActionPacket>())
        {
            PlayerHand hand;
            switch (packet.Action)
            {
                case DrawCardAction:
                    hand = OtherPlayers[packet.PlayerName];
                    hand.Cards.Add(DrawStack.Cards.Pop());
                    break;
                case DrawCardResponse response:
                    var c = new InteractableCard(response.Card.Face);
                    c.Position = DrawStack.Position;
                    PlayerHand.Cards.Add(c);
                    break;
                case PlayCardAction:
                    hand = OtherPlayers[packet.PlayerName];
                    var card = hand.Cards.First();
                    card.IsFaceDown = false;
                    hand.Cards.Remove(card);
                    PlayStack.Cards.Push(card);
                    break;
                default:
                    throw new Exception("Unknown action type");
            }
        }

        Camera.Update();
        Camera.SetActive();

        DrawStack.Update();
        PlayStack.Update();
        PlayerHand.Update();

        foreach (var (_, hand) in OtherPlayers)
        {
            hand.Update();
        }

        if (DrawStack.IsClicked)
        {
            Client.Send(new PlayerActionPacket(this.PlayerName, new DrawCardAction()));
            
            /*
            var card = DrawStack.Cards.Pop();
            card.IsFaceDown = false;
            PlayerHand.Cards.Add(card);
            */
        }

        if (Mouse.IsButtonPressed(MouseButton.Left) && PlayerHand.SelectedCard is not null)
        {
            Client.Send(new PlayerActionPacket(this.PlayerName, new PlayCardAction()));

            var card = PlayerHand.Cards.First();
            PlayerHand.Cards.Remove(card);
            PlayStack.Cards.Push(card);
        }

        base.Update();
    }
}