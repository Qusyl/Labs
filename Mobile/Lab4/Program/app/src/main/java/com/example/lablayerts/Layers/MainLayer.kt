package com.example.lablayerts.Layers

import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.offset
import androidx.compose.foundation.layout.size
import androidx.compose.runtime.Composable
import androidx.compose.ui.AbsoluteAlignment
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.layout.ModifierLocalBeyondBoundsLayout
import androidx.compose.ui.unit.dp
import androidx.compose.ui.zIndex
import com.example.lablayerts.ui.theme.ColorBlue
import com.example.lablayerts.ui.theme.ColorGreen
import com.example.lablayerts.ui.theme.ColorLightBlue
import com.example.lablayerts.ui.theme.ColorOrange
import com.example.lablayerts.ui.theme.ColorRed

@Composable
fun MainLayer( modifier: Modifier = Modifier) {
    Column(modifier)
    {
        Column()
        {
            Row()
            {
                Layer(modifier = Modifier.size(200.dp, 240.dp).zIndex(0.0f), color = ColorRed)
                Layer(modifier = Modifier.size(width = 200.dp, height = 140.dp), color = ColorOrange)
            }

        }


        Layer(modifier = Modifier.size(width = 180.dp, height = 320.dp).offset(100.dp), color = ColorBlue)
        Layer(modifier = Modifier.size(width = 180.dp, height = 180.dp).align(AbsoluteAlignment.Right), color = ColorGreen)
        Layer(modifier = Modifier.size(width = 720.dp, height = 90.dp).align(Alignment.End), color = ColorLightBlue)
    }
}
