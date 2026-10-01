package com.example.myapplication.viewModels

import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.setValue
import androidx.lifecycle.ViewModel
import com.example.myapplication.states.NodState

class MainViewModel : ViewModel() {

    var currentState by mutableStateOf(NodState())
        private set

    fun onCountChange(newCount: Int) {
        val old = currentState.numbers
        val resized = when {
            newCount > old.size -> old + List(newCount - old.size) { "" }
            newCount < old.size -> old.take(newCount)
            else -> old
        }
        currentState = currentState.copy(
            count = newCount,
            numbers = resized,
            error = null,
            result = null
        )
    }

    fun onNumberChange(index: Int, value: String) {
        if (value.any { !it.isDigit() })
        {
            return
        }
        val updated = currentState.numbers.toMutableList().apply { this[index] = value }
        currentState = currentState.copy(numbers = updated, error = null, result = null)
    }

    fun onFindClick() {
        val parsed = currentState.numbers.map { it.toIntOrNull() }

        if (parsed.any { it == null }) {
            currentState = currentState.copy(
                result = null,
                error = "Заполните все поля числами"
            )
            return
        }
        val ints = parsed.filterNotNull()
        if (ints.any { it <= 0 }) {
            currentState = currentState.copy(
                result = null,
                error = "Числа должны быть положительными"
            )
            return
        }

        currentState = currentState.copy(result = findNod(ints), error = null)
    }

    private fun findNod(numbers: List<Int>): Int {
        if (numbers.isEmpty()) return 0
        var result = numbers[0]
        for (i in 1 until numbers.size) result = gcd(result, numbers[i])
        return result
    }

    private fun gcd(a: Int, b: Int): Int {
        var x = a; var y = b
        while (y != 0) {
            val t = x % y;
            x = y;
            y = t }
        return x
    }
}