using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 全屏淡入淡出遮罩。纯代码实现，不需要场景物体、Animator 或动画片段。
///
/// 用法一（推荐，自带防重入）：
///     Fade.Instance.LoadSceneWithFade("Scene2");
///
/// 用法二（需要自己控制顺序时）：
///     yield return Fade.Instance.FadeOut();   // 变黑，等它结束
///     SceneManager.LoadScene("Scene2");
///     yield return null;
///     yield return Fade.Instance.FadeIn();    // 变亮，等它结束
/// </summary>
public class Fade : MonoBehaviour
{
    // ---------- 单例 ----------

    static Fade instance;

    /// <summary>场景里没有挂这个脚本也能用：第一次访问时自动创建一个</summary>
    public static Fade Instance
    {
        get
        {
            if (instance == null) CreateInstance();
            return instance;
        }
    }

    static void CreateInstance()
    {
        // 场景里已经有就复用，避免出现两份遮罩
        Fade existing = FindObjectOfType<Fade>();
        if (existing != null)
        {
            instance = existing;
            return;
        }

        GameObject go = new GameObject("Fade (Auto)");
        go.AddComponent<Fade>();     // AddComponent 会立即触发 Awake
    }

    // ---------- 参数 ----------

    [Header("Fade")]
    [Tooltip("淡入 / 淡出默认时长（秒）")]
    public float defaultDuration = 0.5f;

    [Tooltip("遮罩颜色，一般保持黑色")]
    public Color curtainColor = Color.black;

    // ---------- 内部状态 ----------

    Canvas canvas;
    Image curtain;
    bool loading;                // 防止黑屏期间重复触发场景切换

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        BuildCurtain();
        DontDestroyOnLoad(gameObject);
    }

    // ---------- 遮罩构建 ----------

    void BuildCurtain()
    {
        // 旧版本是用 Animator 驱动 alpha 的，残留的动画会每帧覆盖颜色，必须先关掉
        foreach (Animator anim in GetComponentsInChildren<Animator>(true))
            anim.enabled = false;

        // 旧版本残留的全屏黑图先清成透明，避免它一直盖着画面
        foreach (Image old in GetComponentsInChildren<Image>(true))
        {
            Color c = old.color;
            c.a = 0f;
            old.color = c;
        }

        // Canvas：优先复用手工搭好的，没有就自动建
        canvas = GetComponentInChildren<Canvas>(true);
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("FadeCanvas", typeof(RectTransform));
            canvasObj.transform.SetParent(transform, false);
            canvas = canvasObj.AddComponent<Canvas>();
            canvasObj.AddComponent<CanvasScaler>();
        }
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 32000;         // 盖在 UICanvas（0）之上

        // Image：优先复用，没有就自动建
        curtain = null;
        foreach (Image img in GetComponentsInChildren<Image>(true))
        {
            if (img.gameObject != gameObject) { curtain = img; break; }
        }
        if (curtain == null)
        {
            GameObject curtainObj = new GameObject("Curtain", typeof(RectTransform), typeof(Image));
            curtainObj.transform.SetParent(canvas.transform, false);
            curtain = curtainObj.GetComponent<Image>();
        }

        // 铺满全屏
        RectTransform rt = curtain.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.SetAsLastSibling();

        curtain.raycastTarget = false;        // 不拦截鼠标点击
        SetAlpha(0f);                         // 关键：开局必须透明，否则一点 Play 就黑屏
    }

    // ---------- 对外接口 ----------

    /// <summary>变黑（遮住画面）</summary>
    public Coroutine FadeOut(float duration = -1f)
    {
        return StartCoroutine(FadeRoutine(1f, duration));
    }

    /// <summary>变亮（露出画面）</summary>
    public Coroutine FadeIn(float duration = -1f)
    {
        return StartCoroutine(FadeRoutine(0f, duration));
    }

    /// <summary>不做动画，立刻设置遮罩透明度。0 = 透明，1 = 全黑</summary>
    public void SetAlphaImmediate(float alpha)
    {
        if (curtain == null) BuildCurtain();
        StopAllCoroutines();
        loading = false;
        SetAlpha(alpha);
    }

    /// <summary>
    /// 淡出 → 切场景 → 淡入。自带防重入：黑屏期间再按也不会重复切场景。
    /// </summary>
    /// <param name="sceneName">目标场景名</param>
    /// <param name="onSceneLoaded">场景加载完成（物体 Start 已执行）后的回调</param>
    public void LoadSceneWithFade(string sceneName, Action onSceneLoaded = null)
    {
        if (loading) return;
        if (string.IsNullOrEmpty(sceneName)) return;
        StartCoroutine(LoadSceneRoutine(sceneName, onSceneLoaded));
    }

    IEnumerator LoadSceneRoutine(string sceneName, Action onSceneLoaded)
    {
        loading = true;

        yield return FadeRoutine(1f, defaultDuration);      // ① 先黑屏

        SceneManager.LoadScene(sceneName);
        yield return null;                                   // ② 等新场景物体 Start 跑完（Gate 要靠它定位玩家）

        onSceneLoaded?.Invoke();

        yield return FadeRoutine(0f, defaultDuration);      // ③ 再亮起来

        loading = false;
    }

    // ---------- 核心动画 ----------

    IEnumerator FadeRoutine(float targetAlpha, float duration)
    {
        if (curtain == null) BuildCurtain();
        if (duration < 0f) duration = defaultDuration;

        float startAlpha = curtain.color.a;

        if (duration <= 0f)
        {
            SetAlpha(targetAlpha);
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;      // 用 unscaled，暂停 / 慢放也能正常淡
            SetAlpha(Mathf.Lerp(startAlpha, targetAlpha, t / duration));
            yield return null;
        }

        SetAlpha(targetAlpha);
    }

    void SetAlpha(float a)
    {
        if (curtain == null) return;
        Color c = curtainColor;
        c.a = Mathf.Clamp01(a);
        curtain.color = c;
    }
}
