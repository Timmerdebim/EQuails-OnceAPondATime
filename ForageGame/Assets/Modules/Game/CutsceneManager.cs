using UnityEngine;
using System.IO;
using DG.Tweening;
using UnityEngine.UI;
using System;
using Project.Menus;
using System.Threading.Tasks;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance { get; private set; }

    [SerializeField] private Image blackOverlay;
    [SerializeField] private Image[] introImages;
    [SerializeField] private Image[] outroImages;

    private Sequence cutsceneSeq;
    private bool isCutsceneDone = true;
    private bool isLoadingDone = true;
    private Action currentCallback;
    private bool isBusy;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }



    // ------------ Cutscenes ------------

    public void PlayNewGame(Action callback = null)
    {
        PlayCutscene(introImages, SceneGroup.World, callback);
    }

    public void PlayGame(Action callback = null)
    {
        PlayCutscene(null, SceneGroup.World, callback);
    }

    public void QuitToMainMenu(Action callback = null)
    {
        PlayCutscene(null, SceneGroup.MainMenu, callback);
    }

    public void FinishGame(Action callback = null)
    {
        PlayCutscene(outroImages, SceneGroup.MainMenu, callback);
    }

    // ------------ Cutscene Under da hood Functions ------------

    private void PlayCutscene(Image[] images, SceneGroup sceneGroup, Action callback = null)
    {
        isBusy = true;
        isCutsceneDone = false;
        isLoadingDone = false;
        currentCallback = callback;

        cutsceneSeq?.Kill();
        cutsceneSeq = DOTween.Sequence()
        .SetUpdate(true)

        .Append(blackOverlay.DOFade(1, 1).SetEase(Ease.InOutCubic))
        .AppendCallback(() =>
        {
            GameManager.Instance.sceneLoader.FullLoadSceneGroup(sceneGroup, () =>
            {
                isLoadingDone = true;
                CompleteCutscene();
            });
        });

        if (images != null)
        {
            cutsceneSeq.AppendInterval(1);
            foreach (Image image in images)
            {
                cutsceneSeq.Append(image.DOFade(1, 1f).SetEase(Ease.InOutCubic))
                .AppendInterval(3)
                .Append(image.DOFade(0, 1f).SetEase(Ease.InOutCubic));
            }
        }
        cutsceneSeq.AppendInterval(0.5f)
        .AppendCallback(() =>
        {
            isCutsceneDone = true;
            CompleteCutscene();
        });
    }

    private void CompleteCutscene()
    {
        if (isCutsceneDone && isLoadingDone)
        {
            // cutsceneSeq?.Kill();
            // cutsceneSeq = DOTween.Sequence()
            // .SetEase(Ease.InOutCubic)
            // .SetUpdate(true)
            // .Append(blackOverlay.DOFade(0, 1))
            // .AppendCallback(() =>
            // {
            //     currentCallback?.Invoke();
            //     currentCallback = null;
            //     isBusy = false;
            // });

            blackOverlay.DOFade(0, 1)
            .SetEase(Ease.InOutCubic)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                currentCallback?.Invoke();
                currentCallback = null;
                isBusy = false;
            });
        }
    }
}

