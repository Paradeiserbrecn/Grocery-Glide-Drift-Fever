using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

public class ScoreTextWrapper : MonoBehaviour
{
    [SerializeField]private TMP_Text _driftScoreText;
    private List<char> digits = new List<char>(){'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
    private StringBuilder text = new StringBuilder();

    private void Start()
    {
        _driftScoreText = GetComponent<TMP_Text>();
    }

    public void UpdateText(int score)
    {
        int processedScore = score;
        text.Clear();
        while (processedScore > 0)
        {
            text.Insert(0,processedScore%10);
            processedScore /= 10;
        }

        _driftScoreText.text = text.ToString();
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
