using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    private bool _isUpdating = false;

    public bool IsUpdating {
        get { return _isUpdating; }
    }

    [SerializeField] private float slideDuration;
    [SerializeField] private Image healthBar;
    [SerializeField] private Text ratioText;

    private Stats stats_component;
    private float hp;
    private int maxHp;
    private float speed = -1;

    public void HealthBarSetup(Character _character) {
        stats_component = _character.Stats_Component;
        hp = stats_component.Hp;
        maxHp = stats_component.MaxHp;
        
        UpdateHealthBar();
    }

    private void Update() {
        if (stats_component == null)
            return;

        if (maxHp != stats_component.MaxHp)
            maxHp = stats_component.MaxHp;
        
        if (!Mathf.Approximately(hp, (float)stats_component.Hp)) {
            _isUpdating = true;

            var newHp = stats_component.Hp;

            if (speed == -1) {
                speed = Mathf.Abs(hp - newHp) / 100f;
            }

            if (newHp < hp) {
                // While losing hp, clamp the value to stop at the newHp set
                hp = Mathf.Clamp(hp - speed, newHp, maxHp);
            } else {
                // While gaining hp, clamp the value to stop at the newHp
                hp = Mathf.Clamp(hp + speed, hp, newHp);
            }

            UpdateHealthBar(); 
        } else {
            _isUpdating = false;
            speed = -1;
        }
    }

    private void UpdateHealthBar () { 
        float scale = hp / maxHp;
        ratioText.text = hp.ToString("F0") + "/" + maxHp;
        healthBar.transform.localScale = new Vector3(scale, 1f, 1f);
    }
}
