using ImGuiNET;

namespace Uno.Client.Scenes.MainMenu;

internal class SettingsWindow : MenuWindow
{
    float f;
    bool b;

    public override string Title => "Settings";

    protected override void LayoutContent()
    {
        ImGui.Text("Settings");
        ImGui.Separator();

        ImGui.SliderFloat("Test slider", ref f, 0, 1);
        ImGui.Checkbox("Test checkbox", ref b);

        ImGui.Separator();
        if (ImGui.Button("Back"))
        {
            MenuScene.windows.Pop();
        }
    }
}