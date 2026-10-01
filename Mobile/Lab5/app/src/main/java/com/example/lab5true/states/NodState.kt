package com.example.lab5true.states

data class NodState(
    val count: Int = 2,
    val numbers: List<String> = List(2) { "" },
    val result: Int? = null,
    val error: String? = null
)