package com.example.videolab.viewModels

import android.app.Application
import android.media.MediaPlayer
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.setValue
import androidx.lifecycle.AndroidViewModel
import androidx.lifecycle.ViewModel
import com.example.videolab.R
import com.example.videolab.models.Animal

public  class PictureViewModel(app : Application) : AndroidViewModel(app) {
    private var mediaPlayer: MediaPlayer? = null

    var playingId by mutableStateOf(-1)
        private set

    var isPaused by mutableStateOf(false)
        private set

    val animals: List<Animal> = listOf(
        Animal(0, "Кот", R.drawable.cat, R.raw.cat),
        Animal(1, "Поросенок", R.drawable.pig, R.raw.pig),
        Animal(2, "Корова", R.drawable.cow, R.raw.cow),
        Animal(3, "Утка", R.drawable.duck, R.raw.duck),
        Animal(4, "Овца", R.drawable.sheep, R.raw.sheep),
        Animal(5, "Курица", R.drawable.chicken, R.raw.chicken)
    )

    fun onAnimalClick(animal: Animal) {

        if (playingId != animal.id) {
            startNew(animal)
            return
        }

        mediaPlayer?.let { player ->
            if (isPaused) {
                player.start()
                isPaused = false
            } else {
                player.pause()
                isPaused = true
            }
        }
    }

    fun restart(animal: Animal) {
        startNew(animal)
    }

    private fun startNew(animal: Animal) {
        releasePlayer()
        mediaPlayer = MediaPlayer.create(getApplication(), animal.soundRes)?.apply {
            setOnCompletionListener {
                releasePlayer()
            }
            start()
        }
        if (mediaPlayer != null) {
            playingId = animal.id
            isPaused = false
        }
    }

    private fun releasePlayer() {
        mediaPlayer?.release()
        mediaPlayer = null
        playingId = -1
        isPaused = false
    }

    override fun onCleared() {
        super.onCleared()
        releasePlayer()
    }
}