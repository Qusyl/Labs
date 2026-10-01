package com.example.videolab.composables.videos

import androidx.lifecycle.viewmodel.compose.viewModel
import android.widget.VideoView
import androidx.activity.compose.rememberLauncherForActivityResult
import androidx.activity.result.contract.ActivityResultContracts
import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.compose.ui.viewinterop.AndroidView
import com.example.videolab.viewModels.VideoViewModel
import kotlinx.coroutines.delay

@Composable
fun VideoSection(vm: VideoViewModel = viewModel()) {
    var videoView by remember { mutableStateOf<VideoView?>(null) }

    val filePicker = rememberLauncherForActivityResult(
        ActivityResultContracts.OpenDocument()
    ) { uri -> uri?.let { vm.openFile(it) } }


    LaunchedEffect(vm.isPlaying) {
        while (vm.isPlaying) {
            videoView?.let { vm.position = it.currentPosition }
            delay(300)
        }
    }

    Column(
        modifier = Modifier.fillMaxWidth(),
        verticalArrangement = Arrangement.spacedBy(8.dp)
    ) {
        Text("Видеоплеер", style = MaterialTheme.typography.titleLarge)

        // Адрес / URI
        OutlinedTextField(
            value = vm.urlText,
            onValueChange = vm::onUrlChange,
            label = { Text("URL или URI видео") },
            modifier = Modifier.fillMaxWidth()
        )
        Row(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
            Button(onClick = { vm.openUrl() }) { Text("Открыть ссылку") }
            Button(onClick = { filePicker.launch(arrayOf("video/*")) }) { Text("Выбрать файл") }
        }


        AndroidView(
            modifier = Modifier
                .fillMaxWidth()
                .height(220.dp),
            factory = { context ->
                VideoView(context).apply {
                    setOnPreparedListener { vm.duration = it.duration }
                    setOnCompletionListener {
                        vm.isPlaying = false
                        vm.position = vm.duration
                    }
                    videoView = this
                }
            },
            update = { view ->
                val uri = vm.videoUri
                if (uri != null && view.tag != uri) {
                    view.tag = uri
                    view.setVideoURI(uri)
                }
            }
        )


        val progress = if (vm.duration > 0) vm.position / vm.duration.toFloat() else 0f
        LinearProgressIndicator(progress = { progress }, modifier = Modifier.fillMaxWidth())
        Row(
            modifier = Modifier.fillMaxWidth(),
            horizontalArrangement = Arrangement.SpaceBetween
        ) {
            Text(formatTime(vm.position))
            Text(formatTime(vm.duration))
        }


        Row(
            modifier = Modifier.fillMaxWidth(),
            horizontalArrangement = Arrangement.SpaceEvenly,
            verticalAlignment = Alignment.CenterVertically
        ) {
            Button(onClick = {
                if (vm.videoUri != null) {
                    videoView?.start()
                    vm.isPlaying = true
                }
            }) { Text("Старт") }

            Button(onClick = {
                videoView?.pause()
                vm.isPlaying = false
            }) { Text("Пауза") }

            Button(onClick = {
                videoView?.pause()
                videoView?.seekTo(0)
                vm.isPlaying = false
                vm.position = 0
            }) { Text("Стоп") }
        }
    }
}

private fun formatTime(ms: Int): String {
    val totalSec = ms / 1000
    return "%02d:%02d".format(totalSec / 60, totalSec % 60)
}