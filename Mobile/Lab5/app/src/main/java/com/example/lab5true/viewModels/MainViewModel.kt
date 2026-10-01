package com.example.lab5true.viewModels

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import com.example.lab5true.states.NodState

class MainViewModel : ViewModel() {

    private val _state = MutableLiveData(NodState())
    val state: LiveData<NodState> = _state

    private val current: NodState get() = _state.value ?: NodState()

    fun onCountChange(newCount: Int) {
        val old = current.numbers
        val resized = when {
            newCount > old.size -> old + List(newCount - old.size) { "" }
            newCount < old.size -> old.take(newCount)
            else -> old
        }
        _state.value = current.copy(
            count = newCount,
            numbers = resized,
            error = null,
            result = null
        )
    }

    fun onNumberChange(index: Int, value: String) {
        if (value.any { !it.isDigit() }) {return}
        if(value.length > 1 && value.startsWith("0")){
            _state.value = current.copy(numbers = current.numbers, error = "Число не может начинаться с 0", result = 0)
        return
        }


        val updated = current.numbers.toMutableList().apply { this[index] = value }

        _state.value = current.copy(numbers = updated, error = null, result = null)
    }

    fun onFindClick() {
        val parsed = current.numbers.map { it.toIntOrNull() }

        if (parsed.any { it == null }) {
            _state.value = current.copy(
                result = null,
                error = "Заполните все поля числами"
            )
            return
        }

        val ints = parsed.filterNotNull()
        if (ints.any { it <= 0 }) {
            _state.value = current.copy(
                result = null,
                error = "Числа должны быть положительными"
            )
            return
        }
        _state.value = current.copy(result = findNod(ints), error = null)
    }

    private fun findNod(numbers: List<Int>): Int {
        if (numbers.isEmpty()) return 0
        var result = numbers[0]
        for (i in 1 until numbers.size) result = gcd(result, numbers[i])
        return result
    }

    private fun gcd(a: Int, b: Int): Int {
        var x = a; var y = b
        while (y != 0) { val t = x % y; x = y; y = t }
        return x
    }
}