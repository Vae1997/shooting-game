using UnityEngine;

public class Option : MonoBehaviour {

    public GameObject canvas;

    public void OptionClick()
    {
        if (transform.name.Contains("NewGame"))
            canvas.transform.GetChild(3).gameObject.SetActive(true);
        else if(transform.name.Contains("ConnecBtn"))
        {
            canvas.transform.GetChild(0).gameObject.SetActive(true);
            canvas.transform.GetChild(1).gameObject.SetActive(false);
            canvas.transform.GetChild(2).gameObject.SetActive(false);
            canvas.transform.GetChild(10).gameObject.SetActive(true);
        }
        else if(transform.name.Contains("SetBtn"))
        {
            canvas.transform.GetChild(4).gameObject.SetActive(true);
        }
    }
}
