# Scripting API

## `EditorIcons` class (editor-only)

> ⚠️ This class is available only for Editor features.

### `GetIcon()`

```csharp
public static Texture2D GetIcon(string name, bool black = false, bool x2 = false);
```

Gets the texture of a named icon alone.

- **`string name`**: The name of the icon you want to get, as defined from the [Material Icons list](https://fonts.google.com/icons?icon.set=Material+Icons)
- **`bool black = false`**: Do you want to get the black version of the icon?
- **`bool x2 = false`**: Do you want the doubled size of the icon?

### `IconContent()`

```csharp
public static GUIContent IconContent(string iconName, string text, string tooltip, bool black = false, bool x2 = false);
public static GUIContent IconContent(string iconName, string tooltip, bool black = false, bool x2 = false);
public static GUIContent IconContent(string iconName, bool black = false, bool x2 = false);
```

Creates a [`GUIContent`](https://docs.unity3d.com/ScriptReference/GUIContent.html), using the named icon as content's icon.

- **`string iconName`**: The name of the icon you want to get, as defined from the [Material Icons list](https://fonts.google.com/icons?icon.set=Material+Icons)
- **`string text`**: The text of the output [`GUIContent`](https://docs.unity3d.com/ScriptReference/GUIContent.html)
- **`string tooltip`**: The tooltip of the output [`GUIContent`](https://docs.unity3d.com/ScriptReference/GUIContent.html)
- **`bool black = false`**: Do you want to get the black version of the icon?
- **`bool x2 = false`**: Do you want the doubled size of the icon?

### `IconButton()`

```csharp
public static bool IconButton(Rect position, string iconName, GUIContent label);
public static bool IconButton(Rect position, string iconName, string label, string tooltip);
public static bool IconButton(Rect position, string iconName, string label);
public static bool IconButton(Rect position, string iconName);
public static bool IconButton(string iconName, GUIContent label, params GUILayoutOption[] options);
public static bool IconButton(string iconName, string label, string tooltip, params GUILayoutOption[] options);
public static bool IconButton(string iconName, string label, params GUILayoutOption[] options);
public static bool IconButton(string iconName, params GUILayoutOption[] options);
```

Gets the texture of a named icon alone.

- **`Rect position`**: Rectangle on the screen to use for the button.
- **`string iconName`**: The name of the icon you want to get, as defined from the [Material Icons list](https://fonts.google.com/icons?icon.set=Material+Icons).
- **`GUIContent|string label`**: Content|Text for this button
- **`string tooltip`**: Tooltip for this button
- **`GUILayoutOptions[] options`**: Options for the rectangle on the screen to use for the button