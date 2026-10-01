package com.example.myapplication.states

import androidx.compose.runtime.mutableStateOf

data class NodState(
    val count: Int = 2,
    val numbers: List<String> = List(2) { "" },
    val result: Int? = null,
    val error: String? = null
)
