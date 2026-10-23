package com.example.labsecond8

import android.app.Activity
import android.os.Bundle
import android.os.Looper
import android.widget.Button
import android.widget.ImageView

import com.example.lab9part2.R


class MainActivity : Activity() {
    private lateinit var animationView : ImageView

    private lateinit var startButton : Button

    private  lateinit var pauseButton: Button

    private val handler = android.os.Handler(Looper.getMainLooper())

    private var currentFrame = 0
    private var isPlaying = false
    private val frameDelay = 100L

    private val animationRunnable = object : Runnable {
        override fun run() {
            if (!isPlaying) return

            animationView.setImageResource(frames[currentFrame])

            currentFrame = (currentFrame + 1) % frames.size

            handler.postDelayed(this, frameDelay)
        }
    }
    private val frames = intArrayOf(
        R.drawable.frame_1,
        R.drawable.frame_2,
        R.drawable.frame_3,
        R.drawable.frame_4,
        R.drawable.frame_5,
        R.drawable.frame_6,
        R.drawable.frame_7,
        R.drawable.frame_8,
        R.drawable.frame_9,
        R.drawable.frame_10,
        R.drawable.frame_11,
        R.drawable.frame_12,
        R.drawable.frame_13,
        R.drawable.frame_14
    )
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        animationView = findViewById(R.id.animationView)
        startButton = findViewById(R.id.startButton)
        pauseButton = findViewById(R.id.pauseButton)


        animationView.setImageResource(frames[0])

        startButton.setOnClickListener {
            startAnimation()
        }

        pauseButton.setOnClickListener {
            pauseAnimation()
        }
    }

    private fun startAnimation() {
        if (isPlaying) return

        isPlaying = true
        handler.post(animationRunnable)
    }

    private fun pauseAnimation() {
        isPlaying = false
        handler.removeCallbacks(animationRunnable)
    }

    override fun onDestroy() {
        pauseAnimation()
        super.onDestroy()
    }
}


