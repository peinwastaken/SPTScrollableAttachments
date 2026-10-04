using System.Collections.Generic;
using AttachmentScrolling.Config;
using EFT.UI;
using EFT.UI.WeaponModding;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZLinq;

namespace AttachmentScrolling.Components;

public sealed class AttachmentScrollComponent : MonoBehaviour
{
    private static readonly HashSet<AttachmentScrollComponent> LiveComponents = new();

    private DropDownMenu _menu;
    private Transform _screenRoot;
    private RectTransform _menuRect;
    private RectTransform _itemsContainer;
    private RectTransform _noneItemContainer;
    private ScrollRect _scrollRect;
    private GridLayoutGroup _contentGrid;
    private ContentSizeFitter _menuFitter;
    private ContentSizeFitter _contentFitter;
    private bool _initialized;
    private bool _resetAfterLayout;
    private bool _reportedSetupError;

    public static AttachmentScrollComponent Attach(DropDownMenu menu, Transform screenRoot)
    {
        if (menu == null)
        {
            LogError("Could not attach to an attachment dropdown because the screen did not provide a DropDownMenu.");
            return null;
        }

        AttachmentScrollComponent component = menu.GetComponent<AttachmentScrollComponent>();
        if (component == null)
        {
            component = menu.gameObject.AddComponent<AttachmentScrollComponent>();
        }

        component.Initialize(menu, screenRoot);
        return component;
    }

    public static void ApplySettings()
    {
        AttachmentScrollComponent[] components = LiveComponents.AsValueEnumerable()
            .Where(component => component != null && component._initialized)
            .ToArray();

        foreach (AttachmentScrollComponent component in components)
        {
            component.ApplyCurrentSettings();
        }
    }

    public static bool IsScrollSuppressed(ScrollTrigger trigger, PointerEventData eventData)
    {
        if (trigger == null || eventData == null)
        {
            return false;
        }

        return LiveComponents.AsValueEnumerable()
            .Where(component => component != null && component._initialized)
            .Any(component => component.IsPointerOverOpenMenu(trigger, eventData));
    }

    private static void LogError(string message)
    {
        if (Plugin.Logger != null)
        {
            Plugin.Logger.LogError(message);
        }
    }

    private void Initialize(DropDownMenu menu, Transform screenRoot)
    {
        if (_initialized)
        {
            return;
        }

        _menu = menu;
        _screenRoot = screenRoot != null ? screenRoot : menu.transform.root;
        _menuRect = menu.transform as RectTransform;
        _itemsContainer = menu._itemsContainer;
        _noneItemContainer = menu._noneItemContainer;

        if (_menuRect == null)
        {
            FailInitialization("the DropDownMenu has no RectTransform");
            return;
        }

        if (_itemsContainer == null)
        {
            FailInitialization("the DropDownMenu._itemsContainer reference is missing");
            return;
        }

        if (_noneItemContainer == null)
        {
            FailInitialization("the DropDownMenu._noneItemContainer reference is missing");
            return;
        }

        _contentGrid = _itemsContainer.GetComponent<GridLayoutGroup>();
        if (_contentGrid == null)
        {
            FailInitialization("the attachment items container has no GridLayoutGroup");
            return;
        }

        _scrollRect = _menuRect.GetComponent<ScrollRect>();
        if (_scrollRect == null)
        {
            _scrollRect = _menuRect.gameObject.AddComponent<ScrollRect>();
        }

        if (_menuRect.GetComponent<RectMask2D>() == null)
        {
            _menuRect.gameObject.AddComponent<RectMask2D>();
        }

        _menuFitter = _menuRect.GetComponent<ContentSizeFitter>();
        if (_menuFitter != null)
        {
            _menuFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
            _menuFitter.horizontalFit = ContentSizeFitter.FitMode.MinSize;
        }

        _contentFitter = _itemsContainer.GetComponent<ContentSizeFitter>();
        if (_contentFitter == null)
        {
            _contentFitter = _itemsContainer.gameObject.AddComponent<ContentSizeFitter>();
        }

        _scrollRect.content = _itemsContainer;
        _scrollRect.horizontal = false;
        _scrollRect.vertical = true;

        _itemsContainer.anchorMin = new Vector2(0f, 1f);
        _itemsContainer.anchorMax = new Vector2(1f, 1f);
        _itemsContainer.pivot = new Vector2(0.5f, 1f);

        _contentFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        _contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        _noneItemContainer.SetParent(_itemsContainer, false);
        _noneItemContainer.SetAsFirstSibling();

        _contentGrid.padding.top = 5;
        _contentGrid.padding.bottom = 5;
        _contentGrid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;

        _menu.OnMenuOpen += HandleMenuOpen;
        LiveComponents.Add(this);
        _initialized = true;

        ApplyCurrentSettings();
    }

    private void HandleMenuOpen(ModdingScreenSlotView _)
    {
        ResetScrollPosition();
        _resetAfterLayout = true;
    }

    private void ApplyCurrentSettings()
    {
        if (GeneralConfig.ScrollSpeed != null)
        {
            SetScrollSpeed(GeneralConfig.ScrollSpeed.Value);
        }

        if (GeneralConfig.GridColumns != null)
        {
            SetGridColumns(GeneralConfig.GridColumns.Value);
        }

        if (GeneralConfig.ViewHeight != null)
        {
            SetViewHeight(GeneralConfig.ViewHeight.Value);
        }
    }

    public void SetScrollSpeed(float speed)
    {
        if (_scrollRect != null)
        {
            _scrollRect.scrollSensitivity = Mathf.Max(0f, speed);
        }
    }

    public void SetGridColumns(int columns)
    {
        if (_contentGrid == null)
        {
            return;
        }

        _contentGrid.constraintCount = Mathf.Max(1, columns);
        LayoutRebuilder.MarkLayoutForRebuild(_itemsContainer);
    }

    public void SetViewHeight(float height)
    {
        if (_menuRect == null)
        {
            return;
        }

        _menuRect.sizeDelta = new Vector2(_menuRect.sizeDelta.x, Mathf.Max(0f, height));
    }

    private bool IsPointerOverOpenMenu(ScrollTrigger trigger, PointerEventData eventData)
    {
        if (!_initialized || _menu == null || !_menu.Open || !_menu.gameObject.activeInHierarchy)
        {
            return false;
        }

        if (_menuRect == null || !_menuRect.gameObject.activeInHierarchy || trigger == null || !trigger.gameObject.activeInHierarchy)
        {
            return false;
        }

        if (_screenRoot == null || !IsWithinRoot(trigger.transform, _screenRoot))
        {
            return false;
        }

        return RectTransformUtility.RectangleContainsScreenPoint(
            _menuRect,
            eventData.position,
            eventData.enterEventCamera);
    }

    private static bool IsWithinRoot(Transform target, Transform root)
    {
        return target == root || target.IsChildOf(root);
    }

    private void OnEnable()
    {
        if (_initialized)
        {
            _resetAfterLayout = true;
        }
    }

    private void LateUpdate()
    {
        if (!_resetAfterLayout || !_initialized || _scrollRect == null)
        {
            return;
        }

        _resetAfterLayout = false;
        Canvas.ForceUpdateCanvases();
        ResetScrollPosition();
    }

    private void ResetScrollPosition()
    {
        if (_scrollRect == null)
        {
            return;
        }

        _scrollRect.StopMovement();
        _scrollRect.verticalNormalizedPosition = 1f;
    }

    private void OnDisable()
    {
        if (_scrollRect != null)
        {
            _scrollRect.StopMovement();
        }
    }

    private void OnDestroy()
    {
        if (_menu != null)
        {
            _menu.OnMenuOpen -= HandleMenuOpen;
        }

        LiveComponents.Remove(this);
    }

    private bool FailInitialization(string reason)
    {
        if (!_reportedSetupError)
        {
            LogError($"Unable to enable scrollable attachments for '{name}': {reason}. Check the SPT 4.1 attachment dropdown prefab.");
            _reportedSetupError = true;
        }

        return false;
    }
}
