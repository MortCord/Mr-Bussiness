/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
*/
/*
public class PlayerAnimation : MonoBehaviour
{
    [Header("Посилання на компоненти")]
    [Tooltip("Компонент Animator, який керує моделькою (зазвичай знаходиться на дочірньому об'єкті)")]
    [SerializeField] private Animator animator;

    [Tooltip("Скрипт PlayerMovement, з якого ми беремо інформацію про швидкість руху")]
    [SerializeField] private PlayerMovement movement;

    [Header("Налаштування параметрів")]
    [Tooltip("Назва параметру типу Bool в Animator Controller (має бути точно як у вікні Animator)")]
    public string walkingParameterName = "IsWalking";

    void Start()
    {
        // Якщо посилання не задані вручну в інспекторі, шукаємо їх автоматично
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (movement == null)
            movement = GetComponent<PlayerMovement>();

        // Перевірка на помилки при старті
        if (animator == null) Debug.LogWarning("Увага: Animator не знайдено на об'єкті " + gameObject.name);
        if (movement == null) Debug.LogWarning("Увага: Скрипт PlayerMovement не знайдено на " + gameObject.name);
    }

    void Update()
    {
        // Якщо ми знайшли і аніматор, і скрипт руху
        if (animator != null && movement != null)
        {
            // Передаємо стан руху в аніматор
            // Якщо метод IsMoving() повертає true — вмикається анімація ходьби
            animator.SetBool(walkingParameterName, movement.IsMoving());
        }
    }
}
*/