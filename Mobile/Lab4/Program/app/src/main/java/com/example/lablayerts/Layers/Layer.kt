package com.example.lablayerts.Layers

import androidx.compose.foundation.background
import androidx.compose.foundation.border
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.size
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.unit.dp
import com.example.lablayerts.ui.theme.ColorGreen
import com.example.lablayerts.ui.theme.ColorRed

@Composable
fun Layer(color: Color, modifier: Modifier = Modifier) {
    Box(modifier.background(color).border(width = 4.dp, Color.Black))
}