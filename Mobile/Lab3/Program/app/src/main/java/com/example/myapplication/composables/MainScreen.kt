package com.example.myapplication.composables

import androidx.compose.foundation.border
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.PaddingValues
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import com.example.myapplication.ui.theme.Purple40
import com.example.myapplication.viewModels.MainViewModel

@Composable
fun Background(padding : PaddingValues, viewModel : MainViewModel)
{
    Box(Modifier.fillMaxSize().padding(padding))
    {
        Column(modifier = Modifier
            .fillMaxSize()
            .padding(padding)
            .border(width = 2.dp,
                color = Purple40,
                shape = RoundedCornerShape(6.dp)))
        {
            CountDropdown(1, viewModel)
        }
    }
}