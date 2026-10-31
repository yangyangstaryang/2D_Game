using UnityEngine;
using UnityEngine.UI;

// 类似王者荣耀右下角的技能冷却 UI（冲刺、攻击两个圆形按钮）
// 优先使用场景中已存在的按钮（用菜单 Tools/生成冷却UI到场景 生成后可自由拖拽）；
// 场景里没有时才在运行时代码生成（保底，删了也能自动恢复）。
public class SkillCooldownUI : MonoBehaviour
{
    [Header("Skill Cooldown")]
    public Sprite dashIcon;
    public Sprite swordIcon;

    const float ButtonSize = 100f;   // 按钮直径（仅代码生成时的默认值）
    const float Margin = 35f;        // 距屏幕右下角的边距
    const float Gap = 25f;           // 两个按钮的间距

    // 图标颜色：可用时浅灰，冷却中深灰（压暗区分状态）
    static readonly Color IconReadyColor = new Color(0.72f, 0.72f, 0.72f, 1f);
    static readonly Color IconCoolingColor = new Color(0.45f, 0.45f, 0.45f, 0.95f);

    Image dashFill, attackFill;      // 黑色半透明扇形遮罩
    Text dashText, attackText;       // 倒计时数字
    Image dashIconImg, attackIconImg;
    Text dashLabel, attackLabel;     // 图标缺失时显示的文字

    void Awake()
    {
        LoadIcons();
        if (!HookExisting())
            BuildAll();
    }

    void LoadIcons()
    {
        if (dashIcon == null) dashIcon = Resources.Load<Sprite>("UI/DashIcon");
        if (swordIcon == null) swordIcon = Resources.Load<Sprite>("UI/SwordIcon");
    }

    // 场景里已经有按钮时，按名字把引用接上，不重新生成
    bool HookExisting()
    {
        Transform attack = transform.Find("AttackCDButton");
        Transform dash = transform.Find("DashCDButton");
        if (attack == null || dash == null) return false;

        attackFill = Get<Image>(attack, "CooldownFill");
        attackText = Get<Text>(attack, "Countdown");
        attackIconImg = Get<Image>(attack, "Icon");
        attackLabel = Get<Text>(attack, "Label");
        dashFill = Get<Image>(dash, "CooldownFill");
        dashText = Get<Text>(dash, "Countdown");
        dashIconImg = Get<Image>(dash, "Icon");
        dashLabel = Get<Text>(dash, "Label");

        return attackFill != null && attackText != null &&
               dashFill != null && dashText != null;
    }

    T Get<T>(Transform root, string childName) where T : Component
    {
        Transform t = root.Find(childName);
        return t != null ? t.GetComponent<T>() : null;
    }

    // 生成两个按钮（编辑器菜单可调用；运行时也作为保底）
    public void BuildAll()
    {
        LoadIcons();

        // 先清掉同名的旧按钮，避免重复生成叠加
        DeleteChild("AttackCDButton");
        DeleteChild("DashCDButton");

        Sprite circle = MakeCircleSprite(128);

        // 攻击按钮在右下角，冲刺按钮在它左边
        BuildButton("AttackCDButton",
            new Vector2(-(Margin + ButtonSize / 2f), Margin + ButtonSize / 2f),
            circle, swordIcon, out attackIconImg, out attackFill, out attackText, out attackLabel, "攻击");
        BuildButton("DashCDButton",
            new Vector2(-(Margin + ButtonSize + Gap + ButtonSize / 2f), Margin + ButtonSize / 2f),
            circle, dashIcon, out dashIconImg, out dashFill, out dashText, out dashLabel, "冲刺");
    }

    void DeleteChild(string childName)
    {
        Transform t = transform.Find(childName);
        if (t != null) DestroyImmediate(t.gameObject);
    }

    // 搭一个按钮：白色圆环 + 白色半透明圆底 + 图标 + 扇形遮罩 + 倒计时数字
    void BuildButton(string objName, Vector2 centerPos, Sprite circle, Sprite icon,
                     out Image iconImg, out Image fill, out Text countText, out Text label, string labelName)
    {
        GameObject root = new GameObject(objName, typeof(RectTransform));
        RectTransform rootRt = root.GetComponent<RectTransform>();
        rootRt.SetParent(transform, false);
        rootRt.anchorMin = rootRt.anchorMax = new Vector2(1f, 0f);   // 锚定画布右下角
        rootRt.pivot = new Vector2(0.5f, 0.5f);
        rootRt.sizeDelta = new Vector2(ButtonSize, ButtonSize);
        rootRt.anchoredPosition = centerPos;

        // 白色外环
        CreateChildImage(root.transform, "Ring", circle, new Color(1f, 1f, 1f, 0.95f), 0f);

        // 白色半透明圆底
        CreateChildImage(root.transform, "Bg", circle, new Color(1f, 1f, 1f, 0.4f), 4f);

        // 图标（浅灰色，避免在白色底上消失）
        iconImg = CreateChildImage(root.transform, "Icon",
            icon != null ? icon : circle,
            icon != null ? IconReadyColor : new Color(1f, 1f, 1f, 0.15f), 16f);

        // 图标缺失时用文字代替
        label = null;
        if (icon == null)
        {
            label = CreateChildText(root.transform, "Label", labelName, 18, new Color(1f, 1f, 1f, 0.85f));
            RectTransform lrt = label.rectTransform;
            lrt.anchorMin = new Vector2(0f, 0f);
            lrt.anchorMax = new Vector2(1f, 0f);
            lrt.offsetMin = new Vector2(0f, 6f);
            lrt.offsetMax = new Vector2(0f, 26f);
        }

        // 冷却遮罩（随时间扇形收缩）
        fill = CreateChildImage(root.transform, "CooldownFill", circle, new Color(0f, 0f, 0f, 0.7f), 4f);
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Radial360;
        fill.fillClockwise = true;
        fill.fillOrigin = 0;
        fill.fillAmount = 0f;
        fill.gameObject.SetActive(false);

        // 倒计时数字
        countText = CreateChildText(root.transform, "Countdown", "", 34, Color.white);
        RectTransform trt = countText.rectTransform;
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one;
        trt.offsetMin = Vector2.zero;
        trt.offsetMax = Vector2.zero;
        Outline outline = countText.gameObject.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2f, -2f);
    }

    void Update()
    {
        PlayerController pc = PlayerController.Instance;
        if (pc == null) return;
        ApplyCooldown(attackFill, attackText, attackIconImg, pc.AttackCooldownRemaining, pc.AttackCooldownTotal);
        ApplyCooldown(dashFill, dashText, dashIconImg, pc.DashCooldownRemaining, pc.DashCooldownTotal);
    }

    // 倒计时规则：剩余 >1 秒显示整秒，最后 1 秒按 0.1 递减，转好后遮罩和数字一起消失
    void ApplyCooldown(Image fill, Text text, Image icon, float remaining, float total)
    {
        bool cooling = remaining > 0f;
        if (fill.gameObject.activeSelf != cooling) fill.gameObject.SetActive(cooling);
        if (text.gameObject.activeSelf != cooling) text.gameObject.SetActive(cooling);
        if (icon != null)
            icon.color = cooling ? IconCoolingColor : IconReadyColor;   // 冷却中图标变深灰
        if (!cooling) return;

        fill.fillAmount = total > 0f ? remaining / total : 0f;
        // 向下取整：3.5 秒从"3"开始倒数，不会一上来就显示 4
        text.text = remaining >= 1f ? Mathf.FloorToInt(remaining).ToString() : remaining.ToString("F1");
    }

    // UIController 在 Inspector 里指定了图标时，运行时替换
    public void ApplyIcons()
    {
        if (dashIconImg != null && dashIcon != null)
        {
            dashIconImg.sprite = dashIcon;
            dashIconImg.color = IconReadyColor;
            if (dashLabel != null) dashLabel.gameObject.SetActive(false);
        }
        if (attackIconImg != null && swordIcon != null)
        {
            attackIconImg.sprite = swordIcon;
            attackIconImg.color = IconReadyColor;
            if (attackLabel != null) attackLabel.gameObject.SetActive(false);
        }
    }

    Image CreateChildImage(Transform parent, string objName, Sprite sprite, Color color, float inset)
    {
        GameObject go = new GameObject(objName, typeof(RectTransform));
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(inset, inset);
        rt.offsetMax = new Vector2(-inset, -inset);
        Image img = go.AddComponent<Image>();
        img.sprite = sprite;
        img.color = color;
        img.raycastTarget = false;
        return img;
    }

    Text CreateChildText(Transform parent, string objName, string content, int fontSize, Color color)
    {
        GameObject go = new GameObject(objName, typeof(RectTransform));
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        Text t = go.AddComponent<Text>();
        t.font = BuiltinFont;
        t.text = content;
        t.fontSize = fontSize;
        t.color = color;
        t.alignment = TextAnchor.MiddleCenter;
        t.raycastTarget = false;
        return t;
    }

    static Font builtinFont;
    static Font BuiltinFont
    {
        get
        {
            if (builtinFont == null)
            {
                try { builtinFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); } catch { }
                if (builtinFont == null)
                {
                    try { builtinFont = Resources.GetBuiltinResource<Font>("Arial.ttf"); } catch { }
                }
            }
            return builtinFont;
        }
    }

    // 程序化生成一个边缘抗锯齿的白色圆形贴图（不依赖任何外部图片资源）
    static Sprite MakeCircleSprite(int size)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        float r = size / 2f - 1f;
        Vector2 center = new Vector2(size / 2f, size / 2f);
        Color[] pixels = new Color[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center);
                float a = Mathf.Clamp01(r - d + 1f);
                pixels[y * size + x] = new Color(1f, 1f, 1f, a);
            }
        }
        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
    }
}
