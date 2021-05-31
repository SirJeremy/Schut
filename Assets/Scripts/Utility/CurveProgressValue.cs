using UnityEngine;

//common untility script I made and use
//great for jank programmer animations
[System.Serializable]
public class CurveProgressValue {
    #region Variables
    [SerializeField] [Range(0, 1)]
    private float progress = 0;
    [SerializeField]
    private float progressSpeed = 1;
    [SerializeField]
    private AnimationCurve curve;
    [SerializeField]
    private float minOutputValue = 0;
    [SerializeField]
    private float maxOutputValue = 1;
    [SerializeField]
    private bool outputValueRangeIsAbsolute = true;

    private float value = 0;
    private bool isDirty = true;
    #endregion

    #region Properties
    public float Progress {
        get { return progress; }
        set {
            if (progress == value)
                return;
            if(value < 0)
                progress = 0;
            else if(value > 1)
                progress = 1;
            else
                progress = value;
            isDirty = true;
        }
    }
    public float ProgressSpeed {
        get { return progressSpeed; }
        set { progressSpeed = value; isDirty = true; }
    }
    public float MinOutputValue {
        get { return minOutputValue; }
        set { minOutputValue = value; }
    }
    public float MaxOutputValue {
        get { return maxOutputValue; }
        set { maxOutputValue = value; isDirty = true; }
    }
    public bool OutputValueRangeIsAbsolute {
        get { return outputValueRangeIsAbsolute; }
        set { outputValueRangeIsAbsolute = value; isDirty = true; }
    }
    public AnimationCurve Curve {
        get { return curve; }
    }
    public float RawValue {
        get { return curve.Evaluate(progress); }
    }
    public float Value {
        get {
            if (!isDirty)
                return value;
            if(OutputValueRangeIsAbsolute)
                value = Mathf.Lerp(minOutputValue, maxOutputValue, curve.Evaluate(progress));
            else
                value = Mathf.LerpUnclamped(minOutputValue, maxOutputValue, curve.Evaluate(progress));
            isDirty = false;
            return value;
        }
    }
    public bool IsMinProgress {
        get { return progress == 0; }
    }
    public bool IsMaxProgress {
        get { return progress == 1; }
    }
    #endregion

    #region Constructors
    public CurveProgressValue() {
        ProgressSpeed = 1;
        MinOutputValue = 0;
        MaxOutputValue = 1;
        OutputValueRangeIsAbsolute = true;
        isDirty = true;
    }
    public CurveProgressValue(float progressSpeed = 1, float minOutputValue = 0, float maxOutputValue = 1, bool outputValueRangeIsAbsolute = true) {
        ProgressSpeed = progressSpeed;
        MinOutputValue = minOutputValue;
        MaxOutputValue = maxOutputValue;
        OutputValueRangeIsAbsolute = outputValueRangeIsAbsolute;
        isDirty = true;
    }
    #endregion

    #region Methods
    public void IncrementProgress() {
        Progress += Time.deltaTime * ProgressSpeed;
    }
    public void IncrementProgress(float deltaTime) {
        Progress += Time.deltaTime * ProgressSpeed;
    }
    public void DecrementProgress() {
        Progress -= Time.deltaTime * ProgressSpeed;
    }
    public void DecrementProgress(float deltaTime) {
        Progress -= Time.deltaTime * ProgressSpeed;
    }
    public float GetValueAndIncrementProgress() {
        IncrementProgress();
        return Value;
    }
    public float GetValueAndIncrementProgress(float deltaTime) {
        IncrementProgress(deltaTime);
        return Value;
    }
    public float GetValueAndDecrementProgress() {
        DecrementProgress();
        return Value;
    }
    public float GetValueAndDecrementProgress(float deltaTime) {
        DecrementProgress(deltaTime);
        return Value;
    }
    public void SetToMinProgress() {
        Progress = 0;
    }
    public void SetToMaxProgress() {
        Progress = 1;
    }
    #endregion
}
