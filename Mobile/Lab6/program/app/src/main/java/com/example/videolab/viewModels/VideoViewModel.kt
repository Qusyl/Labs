package com.example.videolab.viewModels

import android.app.Application
import android.net.Uri
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.setValue
import androidx.lifecycle.AndroidViewModel

class VideoViewModel(app: Application) : AndroidViewModel(app) {

    var urlText by mutableStateOf("")
    var videoUri by mutableStateOf<Uri?>(null)
        private set

    var isPlaying by mutableStateOf(false)
    var position by mutableStateOf(0)
    var duration by mutableStateOf(0)

    fun onUrlChange(text: String) {
        urlText = text
    }


    fun openUrl() {
        if (urlText.isNotBlank()) setSource(Uri.parse(urlText.trim()))
    }


    fun openFile(uri: Uri) {
        urlText = uri.toString()
        setSource(uri)
    }

    private fun setSource(uri: Uri) {
        videoUri = uri
        isPlaying = false
        position = 0
        duration = 0
    }
}