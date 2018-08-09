﻿using System.Collections;
using UnityEngine;

public class UpdatableData :ScriptableObject {
  public event System.Action OnValuesUpdated;
  public bool autoUpdate = true;

#if UNITY_EDITOR
  protected virtual void OnValidate() {
    if (autoUpdate)
      UnityEditor.EditorApplication.update += NotifyOfUpdatedValues;
  }

  public void NotifyOfUpdatedValues() {
    UnityEditor.EditorApplication.update -= NotifyOfUpdatedValues;
    OnValuesUpdated?.Invoke();
  }
#endif

  public virtual void NotifyUpdate() {
    OnValuesUpdated?.Invoke();
  }
}
