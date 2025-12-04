using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

public class ScoreTextWrapper : MonoBehaviour
{
    [SerializeField]private TMP_Text _driftScoreText;

    private void Start()
    {
        _driftScoreText = GetComponent<TMP_Text>();
    }

    public void UpdateText(int score)
    {

        _driftScoreText.text = score.ToString();
    }

    public string GetText()
    {
        return _driftScoreText.text;
    }

    public void SetPos(Vector3 position)
    {
        transform.position = position;
    }

    public void SetSize(float size)
    {
        transform.localScale = new Vector3(size, size, size);
    }

    public void SetColor(Color color)
    {
        _driftScoreText.color = color;
    }
}
