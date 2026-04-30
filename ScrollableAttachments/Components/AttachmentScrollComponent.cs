using AttachmentScrolling.Config;
using EFT.UI;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace AttachmentScrolling.Components;

public class AttachmentScrollComponent : MonoBehaviour
{
    public static AttachmentScrollComponent Instance;
    
    private ScrollRect _scrollRect;
    private GridLayoutGroup _contentGrid;
    private RectTransform _dropDownRect;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        
        Instance = this;
        
        Transform previewPanel = GameObject.Find("Preview Panel").transform;
        Transform dropDown = previewPanel.transform.Find("DropdownMenu");

        Transform emptyItem = dropDown.Find("ItemViewContainer");
        Transform container = dropDown.Find("Container");
        
        RectTransform containerRect = container.GetComponent<RectTransform>();

        // reposition empty item to be inside our scroll content instead
        emptyItem.parent = container;
        emptyItem.SetSiblingIndex(0);
        
        // set up rect mask and scroll rect
        RectMask2D rectMask = dropDown.gameObject.AddComponent<RectMask2D>();
        ScrollRect scrollRect = dropDown.gameObject.AddComponent<ScrollRect>();
        scrollRect.content = containerRect;
        scrollRect.scrollSensitivity = GeneralConfig.ScrollSpeed.Value;
        scrollRect.horizontal = false;
        
        // update dropdown size fitter
        ContentSizeFitter fitter = dropDown.GetComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.horizontalFit = ContentSizeFitter.FitMode.MinSize;
        
        // update dropdown content grid
        GridLayoutGroup contentGrid = container.GetComponent<GridLayoutGroup>();
        contentGrid.padding.top = 5;
        contentGrid.padding.bottom = 5;
        contentGrid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        contentGrid.constraintCount = GeneralConfig.GridColumns.Value;
        
        // fix dropdown rect size
        RectTransform rect = dropDown.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, GeneralConfig.ViewHeight.Value);

        _scrollRect = scrollRect;
        _contentGrid = contentGrid;
        _dropDownRect = rect;
    }

    public void SetScrollSpeed(float speed)
    {
        _scrollRect.scrollSensitivity = speed;
    }

    public void SetGridColumns(int cols)
    {
        _contentGrid.constraintCount = cols;
    }

    public void SetViewHeight(float height)
    {
        _dropDownRect.sizeDelta = new Vector2(_dropDownRect.sizeDelta.x, height);
    }
}
