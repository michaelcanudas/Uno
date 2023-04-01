using SimulationFramework.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Uno.Actions;
using Uno.Client.MainMenu;
using Uno.Packets;

namespace Uno.Client.Gameplay;
internal class GameplayScene : GameScene
{
    // this player
    public string CurrentPlayerName { get; set; }

    public Dictionary<string, PlayerHand> Hands { get; set; } = new();

    // stack where players play cards onto
    public InteractableCardStack PlayStack { get; set; }
    
    // stack where players draw cards from
    public InteractableCardStack DrawStack { get; set; }
    public Camera Camera;
    
    private ColorSelectWindow? colorSelectWindow;
    private ActionsBar actionsBar;
    private EscMenu? escMenu;

    public GameplayScene(string playerName, List<string> allPlayers)
    {
        CurrentPlayerName = playerName;
        Camera = new(4);

        PlayStack = new() { Position = new(0, 0), Opaque = false };
        DrawStack = new() { Position = new(-1, 0), Opaque = true };

        Hands.Add(playerName, new(Enumerable.Empty<InteractableCard>()) { SelectionEnabled = true, Position = new(0, 2), Rotation = Angle.ToRadians(0), Scale = 1f });

        foreach (var player in allPlayers)
        {
            if (player == playerName)
                continue;

            Hands.Add(player, new(Enumerable.Empty<InteractableCard>()) { Position = Vector2.UnitY * -3.5f, Rotation = Angle.ToRadians(180), Scale = .5f });
        }

        actionsBar = new();
    }

    public override void Render(ICanvas canvas)
    {
        Camera.ApplyTo(canvas);

        DrawStack.Render(canvas);
        PlayStack.Render(canvas);

        foreach (var (_, hand) in Hands)
        {
            hand.Render(canvas);
        }

        base.Render(canvas);
    }

    public override void Update()
    {
        if (Keyboard.IsKeyPressed(Key.Esc))
        {
            if (escMenu is null)
            {
                escMenu = new();
            }
            else
            {
                escMenu = null;
            }
        }

        actionsBar.Layout();
        colorSelectWindow?.Layout();
        escMenu?.Layout();

        foreach (var packet in Client.Receive<PlayerActionPacket>())
        {
            PlayerHand hand;
            switch (packet.Action)
            {
                case DrawCardAction.Response response:
                    var c = new InteractableCard(response.Card) { Position = DrawStack.Position };
                    Hands[packet.PlayerName].Cards.Add(c);
                    break;
                case PlayCardAction.Response response:
                    hand = Hands[packet.PlayerName];
                    var card = hand.GetCard(response.PlayedCard) ?? throw new();
                    hand.Cards.Remove(card);
                    card.Card = response.PlayedCard;
                    PlayStack.Cards.Push(card);
                    break;
                default:
                    throw new Exception("Unknown action type");
            }
        }

        Camera.Update();
        Camera.SetActive();

        foreach (var (_, hand) in Hands)
        {
            hand.Update();
        }

        DrawStack.Update();
        PlayStack.Update();

        if (DrawStack.IsClicked)
        {
            Client.Send(new PlayerActionPacket(this.CurrentPlayerName, new DrawCardAction()));
        }

        var playerHand = Hands[CurrentPlayerName];
        if (Mouse.IsButtonPressed(MouseButton.Left) && playerHand.SelectedCard is not null)
        {
            Client.Send(new PlayerActionPacket(this.CurrentPlayerName, new PlayCardAction() { Card = playerHand.SelectedCard.Card }));
        }

        base.Update();
    }
}