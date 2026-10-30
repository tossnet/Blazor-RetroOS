namespace Blazor.RetroOS.Pages;

// The code samples are here and not in an @code block: Razor would parse their <script> and <?php ... ?> as markup.
public partial class StaticCssPage
{
    private const string CdnCode = """
        <link href="https://cdn.jsdelivr.net/gh/tossnet/Blazor-RetroOS@main/static/dist/retroos-amiga13.css" rel="stylesheet" />
        """;

    private const string TreeCode = """
        my-site/
        ├── index.html (or index.php)
        └── assets/
            └── retroos/
                ├── retroos-amiga13.css
                ├── retroos-atarist.css
                ├── retroos.css
                └── fonts/
        """;

    private const string LinkCode = """
        <link id="retro-theme" href="assets/retroos/retroos-amiga13.css" rel="stylesheet" />
        """;

    private const string SwitchCode = """
        <script>
            document.getElementById('retro-theme').href = 'assets/retroos/retroos-atarist.css';
        </script>
        """;

    private const string BundleCode = """
        <link href="assets/retroos/retroos.css" rel="stylesheet" />

        <div class="retro-screen retro-appearance-atarist">
            <button class="retro-button retro-appearance-amiga13">OK</button>
        </div>
        """;

    private const string ComponentsCode = """
        <div class="retro-window retro-window-active" role="dialog" aria-label="Hello">
            <div class="retro-titlebar">
                <button type="button" class="retro-gadget retro-gadget-close" aria-label="Close"></button>
                <span class="retro-titlebar-text">Hello</span>
                <button type="button" class="retro-gadget retro-gadget-back" aria-label="Send to back"></button>
                <button type="button" class="retro-gadget retro-gadget-front" aria-label="Bring to front"></button>
            </div>
            <div class="retro-window-body">
                <label class="retro-checkbox">
                    <input type="checkbox" class="retro-checkbox-input" />
                    <span>Remember me</span>
                </label>
                <button type="button" class="retro-button retro-button-default">OK</button>
                <button type="button" class="retro-button">Cancel</button>
            </div>
        </div>
        """;

    private const string PhpCode = """
        <?php
        function retro_button(string $text, bool $default = false): string
        {
            $class = 'retro-button' . ($default ? ' retro-button-default' : '');
            return '<button type="button" class="' . $class . '">' . htmlspecialchars($text) . '</button>';
        }
        ?>

        <?= retro_button('OK', true) ?>
        <?= retro_button('Cancel') ?>
        """;
}
