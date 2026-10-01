package com.example.videolab.composables.pictures

import androidx.compose.foundation.Image
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.grid.GridCells
import androidx.compose.foundation.lazy.grid.GridItemSpan
import androidx.compose.foundation.lazy.grid.LazyVerticalGrid
import androidx.compose.foundation.lazy.grid.items
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Clear
import androidx.compose.material.icons.filled.PlayArrow
import androidx.compose.material.icons.filled.Refresh
import androidx.compose.material3.*
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.lifecycle.viewmodel.compose.viewModel
import com.example.videolab.composables.videos.VideoSection
import com.example.videolab.models.Animal

import com.example.videolab.viewModels.PictureViewModel

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun MainPictureScreen(viewModel: PictureViewModel = viewModel()) {

    Scaffold(
        topBar = { TopAppBar(title = { Text("Голоса живых существ") }) }
    ) { padding ->
        LazyVerticalGrid(
            columns = GridCells.Fixed(2),
            modifier = Modifier
                .padding(padding)
                .fillMaxSize()
                .padding(8.dp),
            horizontalArrangement = Arrangement.spacedBy(8.dp),
            verticalArrangement = Arrangement.spacedBy(8.dp)
        ) {
            items(viewModel.animals, key = { it.id }) { animal ->
                AnimalCard(
                    animal = animal,
                    isCurrent = viewModel.playingId == animal.id,
                    isPaused = viewModel.playingId == animal.id && viewModel.isPaused,
                    onPlayPause = { viewModel.onAnimalClick(animal) },
                    onRestart = { viewModel.restart(animal) }
                )
            }
            item(span = { GridItemSpan(maxLineSpan) }){
                VideoSection()
            }
        }
    }
}

@Composable
private fun AnimalCard(
    animal: Animal,
    isCurrent: Boolean,
    isPaused: Boolean,
    onPlayPause: () -> Unit,
    onRestart: () -> Unit
) {
    Card(
        modifier = Modifier
            .fillMaxWidth()
            .clickable { onPlayPause() },
        colors = CardDefaults.cardColors(
            containerColor = if (isCurrent)
                MaterialTheme.colorScheme.primaryContainer
            else
                MaterialTheme.colorScheme.surfaceVariant
        )
    ) {
        Column(
            modifier = Modifier.padding(8.dp),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            Image(
                painter = painterResource(animal.imageRes),
                contentDescription = animal.name,
                modifier = Modifier
                    .fillMaxWidth()
                    .height(120.dp)
            )
            Spacer(Modifier.height(6.dp))
            Text(animal.name, fontWeight = FontWeight.Bold)

            Spacer(Modifier.height(6.dp))
            Row(verticalAlignment = Alignment.CenterVertically) {

                IconButton(onClick = onPlayPause) {
                    Icon(
                        imageVector = if (isCurrent && !isPaused)
                            Icons.Default.Clear
                        else
                            Icons.Default.PlayArrow,
                        contentDescription = if (isCurrent && !isPaused) "Пауза" else "Играть"
                    )
                }

                IconButton(onClick = onRestart) {
                    Icon(Icons.Default.Refresh, contentDescription = "Заново")
                }
            }
        }
    }
}