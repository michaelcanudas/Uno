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

    public ChatWindow chatWindow;
    private ColorSelectWindow? colorSelectWindow;
    private ActionsBar actionsBar;
    private EscMenu? escMenu;

    public GameplayScene(string playerName, StartPacket initialState)
    {
        CurrentPlayerName = playerName;
        Camera = new(4);

        PlayStack = new() { Position = new(0, 0), Opaque = false };
        DrawStack = new() { Position = new(-1, 0), Opaque = true };

        Hands.Add(playerName, new(initialState.GetStartingHand(playerName).Select(c => new InteractableCard(c))) { SelectionEnabled = true, Position = new(0, 2) });
        PlayStack.Cards.Push(new(initialState.StartingDiscard));

        var count = initialState.Players.Length - 1;
        float breadth = 3f;
        float increment = breadth / count;
        float baseX = (increment - breadth) / 2f;

        float x = baseX;
        foreach (var player in initialState.Players)
        {
            if (player == playerName)
                continue;

            Hands.Add(player, new(initialState.GetStartingHand(player).Select(c => new InteractableCard(c))) { Position = new(x, -3.75f), Rotation = Angle.ToRadians(180), Scale = .5f });
            x += increment;
        }

        actionsBar = new();
        chatWindow = new();
    }

    public override void Render(ICanvas canvas)
    {
        Camera.ApplyTo(canvas);

        DrawStack.Render(canvas);
        PlayStack.Render(canvas);

        foreach (var (name, hand) in Hands)
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

        foreach (var (name, hand) in Hands)
        {
            if (name == CurrentPlayerName)
                continue;

            ImGui.PushID(name);
            ImGui.SetNextWindowPos(new(Camera.WorldToScreen(hand.Position).X + 40, 120));
            if (ImGui.Begin("##window", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.AlwaysAutoResize))
            {
                ImGui.Text(name);
            }
            ImGui.End();
            ImGui.PopID();
        }

        actionsBar.Layout();
        chatWindow.Layout();
        colorSelectWindow?.Layout();
        escMenu?.Layout();

        if (escMenu is not null && escMenu.WantClose)
        {
            escMenu = null;
        }

        foreach (var packet in Client.Receive<PlayerActionPacket>())
        {
            PlayerHand hand = Hands[packet.PlayerName];
            switch (packet.Action)
            {
                case DrawCardAction.Response response:
                    hand.Cards.AddRange(
                        response.Cards.Select(
                            c => new InteractableCard(c) { Position = DrawStack.Position }
                            )
                        );
                    break;
                case PlayCardAction.Response response:
                    var card = hand.GetCard(response.PlayedCard) ?? throw new();
                    hand.Cards.Remove(card);
                    card.Card = response.PlayedCard;
                    PlayStack.Cards.Push(card);

                    if (card.Face.Kind.IsWild() && packet.PlayerName == this.CurrentPlayerName)
                    {
                        // we need to select a color!! server will wait for us, open window now.
                        colorSelectWindow = new();
                    }
                    break;
                case SelectColorAction.Response response:
                    var wildCard = PlayStack.Cards.Peek();
                    wildCard.Card.Face = new(wildCard.Face.Kind, response.Color);
                    if (packet.PlayerName == this.CurrentPlayerName)
                        this.colorSelectWindow = null; // our color select went through
                    break;
                case ChatMessageAction chatMessage:
                    chatWindow.AddMessage(packet.PlayerName, chatMessage.Message);
                    break;
                default:
                    throw new Exception("Unknown action type");
            }
        }

        foreach (var notification in Client.Receive<ServerNotificationPacket>())
        {
            chatWindow.AddNotification(notification.Notification);
        }

        Camera.Update();
        Camera.SetActive();

        foreach (var (_, hand) in Hands)
        {
            hand.Update();
        }

        DrawStack.Update();
        PlayStack.Update();

        if (chatWindow.wantSendMessage)
        {
            Client.Send(new PlayerActionPacket(this.CurrentPlayerName, new ChatMessageAction() { Message = chatWindow.CurrentMessage ?? "" }));
        }

        if (colorSelectWindow is not null)
        {
            colorSelectWindow.Layout();

            if (colorSelectWindow.IsColorSelected)
            {
                var action = new SelectColorAction()
                {
                    Color = colorSelectWindow.SelectedColor.Value
                };

                Client.Send(new PlayerActionPacket(this.CurrentPlayerName, action));
                colorSelectWindow = null;
            }

            return;
        }

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