# E-Service — C# / Avalonia rewrite

This is a full rewrite of the original Java/NetBeans/Swing **E-Service**
restaurant point-of-sale project. It's the **same app, same features,
same `.txt` file storage** — the goal was a modern-looking desktop UI and
cleaner code, not a redesign of what the program actually does.

## How to build and run

You need the [.NET 8 SDK](https://dotnet.microsoft.com/download) installed
(you already have it — this was set up earlier).

```bash
cd EService
dotnet restore   # downloads Avalonia and its dependencies from NuGet
dotnet run       # builds and launches the app
```

Open the folder in VS Code (`code .`) to browse/edit — you already have
the C# extension set up from before.

## Where does my data get saved now?

The original hardcoded Windows paths like `C:\EserviceData\...`. Since
this now runs on Linux (and could run on Windows/Mac too), data is stored
in the standard per-user application-data folder instead:

- **Linux:** `~/.local/share/EService/`
- **Windows:** `%APPDATA%\EService\`

Inside that folder:
- `Data/users.txt`, `Data/insumos.txt`, `Data/transactions.txt` — same
  `|`-delimited plain text format the original used
- `Pedidos/` — one `.txt` receipt per confirmed order
- `Facturas/` — one `.txt` invoice per stock purchase

Nothing about the file *format* changed — only where the files live, so
it works correctly no matter whose computer it's running on.

## Project structure (and what each old file became)

The project follows the **MVVM pattern** (Model-View-ViewModel), which is
the standard way desktop apps are built in C#. It's a more formal version
of the split your original project already had between `clases` (data) and
`controladores` (logic) — here it's just one step more organized, with a
third layer (`ViewModels`) sitting between your data/logic and the actual
screens, so the screens themselves stay simple.

```
EService/
├── Models/          → same as your old "clases" folder
├── Services/         → same as your old "controladores" folder
├── ViewModels/        → NEW: connects each screen to the Services below it
├── Views/              → the actual screens (XAML instead of Swing)
│   └── Dialogs/         → small popup windows (message/confirm/input boxes)
└── Assets/               → dish icons, logo, app icon (copied from Recursos/)
```

### Models (data only, no logic)
| Old Java file | New C# file |
|---|---|
| `Cliente.java` | `Models/Cliente.cs` |
| `Pedido.java` | `Models/Pedido.cs` |
| `Insumo.java` | `Models/Insumo.cs` |
| `Empleado.java` | `Models/Empleado.cs` |
| `Plato.java` | `Models/Plato.cs` |
| `Transaction.java` | `Models/Transaction.cs` |

### Services (business logic + file reading/writing — no UI code)
| Old Java file | New C# file | Notes |
|---|---|---|
| `FileManager.java` | `Services/FileDataStore.cs` | Same `.txt` format, cross-platform paths |
| `LoginController.java` | `Services/LoginService.cs` | Same employee list/validation logic |
| `InsumoController.java` | `Services/InsumoService.cs` | Same default ~74-item ingredient catalog |
| `PlatoController.java` | `Services/PlatoService.cs` | Same 40-item menu + recipes |
| `TransactionController.java` | `Services/TransactionService.cs` | Same sales/purchases ledger |
| `Carrito.java` | `Services/CartService.cs` | Same "current order" cart |
| *(new)* | `Services/DialogService.cs` | Replaces `JOptionPane` popups |
| *(new)* | `Services/NavigationService.cs` | Replaces "open a new JFrame" |
| *(new)* | `Services/AssetImageLoader.cs` | Loads dish icons for the UI |

### Views + ViewModels (the actual screens)
| Old Java screen | New ViewModel | New View (XAML) |
|---|---|---|
| `Beginningpage.java` (login) | `LoginViewModel.cs` | `LoginView.axaml` |
| `lobbyPage.java` (home hub) | `LobbyViewModel.cs` | `LobbyView.axaml` |
| `Order_menu.java` (server ordering) | `OrderMenuViewModel.cs` | `OrderMenuView.axaml` |
| `TerminarCompra.java` (checkout) | `CheckoutViewModel.cs` | `CheckoutView.axaml` |
| `manager01page.java` (dashboard) | `ManagerDashboardViewModel.cs` | `ManagerDashboardView.axaml` |
| `Inventario.java` (stock view) | `InventoryViewModel.cs` | `InventoryView.axaml` |
| `manageragregarcompra.java` (add stock) | `AddPurchaseViewModel.cs` | `AddPurchaseView.axaml` |
| `usuariosManagerPage.java` (manage employees) | `UserManagementViewModel.cs` | `UserManagementView.axaml` |

**How does a "screen" work here?** Each `View` (`.axaml` file) is just the
visual layout — boxes, buttons, text — with no logic in it. Each
`ViewModel` holds the data the screen shows and the actions its buttons
perform. When you click a button in the View, it runs a `Command` on the
ViewModel; when the ViewModel's data changes, the View updates itself
automatically. This "binding" is what replaces all the manual
`label.setText(...)` / `button.addActionListener(...)` code from Swing.

### Only one real window
Unlike the original, which opened a **new window** every time you moved
to another screen (`new Order_menu().setVisible(true)`), this version has
exactly **one window** (`MainWindow`), and screens are swapped inside it.
`NavigationService.NavigateTo(new OrderMenuViewModel())` is the direct
equivalent of that old line — same idea, no extra windows piling up.

## What changed on purpose (modernization)

- **Visual style**: flat, card-based layout with a consistent color
  palette (`Styles.axaml`) instead of default Swing/NetBeans widgets.
- **Menu screen**: the original had 40 individually hand-placed buttons.
  Here, the menu is one data-bound grid generated from the dish list — add
  a new dish to `PlatoService.cs` and it automatically appears in the UI,
  no manual button placement needed.
- **Checkout summary**: groups repeated items ("Coca-Cola x3") instead of
  listing every unit as a separate line.
- **Cross-platform file paths** (see "Where does my data get saved" above).

## What did NOT change

- The `.txt`-file-based storage approach (no database).
- The menu catalog, prices, and recipes (ingredient consumption per dish).
- The default ingredient catalog and seed employee codes.
- The overall flow: log in → (server: order → checkout) or
  (manager: dashboard → inventory / purchases / employees).

## A note on testing

This project was written and organized carefully, but it was **not
compiled or run** in the environment it was written in (no internet
access to NuGet there). The first `dotnet restore && dotnet run` on your
machine is the real first test — if anything doesn't compile, paste the
exact error here and it'll get fixed quickly; XAML/C# typos are common
and easy to sort out once we see the compiler's actual output.
