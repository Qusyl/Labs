package com.example.myapplication.states

import androidx.compose.runtime.mutableStateOf

data class NodState(
    var numbers : MutableList<Int> = mutableListOf(),
    var result : Int = 0
    )
