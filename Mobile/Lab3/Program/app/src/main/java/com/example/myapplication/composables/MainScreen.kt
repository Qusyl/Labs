package com.example.myapplication.composables

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material3.Button
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.OutlinedTextField
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.unit.dp
import androidx.lifecycle.viewmodel.compose.viewModel
import com.example.myapplication.viewModels.MainViewModel

@Composable
fun MainScreen(viewModel: MainViewModel = viewModel()) {
    val state = viewModel.currentState

    Scaffold { padding ->
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(padding)
                .padding(16.dp),
            verticalArrangement = Arrangement.spacedBy(12.dp)
        ) {
            CountDropdown(
                count = state.count,
                onCountChange = viewModel::onCountChange
            )

            state.numbers.forEachIndexed { index, value ->
                OutlinedTextField(
                    value = value,
                    onValueChange = { viewModel.onNumberChange(index, it) },
                    label = { Text("Число ${index + 1}") },
                    singleLine = true,
                    keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Number),
                    modifier = Modifier.fillMaxWidth()
                )
            }


            Button(
                onClick = viewModel::onFindClick,
                modifier = Modifier.fillMaxWidth()
            ) {
                Text("Найти")
            }


            state.error?.let { message ->
                Text(
                    text = message,
                    color = MaterialTheme.colorScheme.error,
                    style = MaterialTheme.typography.bodyMedium
                )
            }
            OutlinedTextField(
                value = state.result?.toString() ?: "",
                onValueChange = {},
                readOnly = true,
                label = { Text("НОД") },
                modifier = Modifier.fillMaxWidth()
            )
        }
    }
}