package com.example.videolab.models

import androidx.annotation.DrawableRes
import androidx.annotation.RawRes

data class Animal(
    val id: Int,
    val name: String,
    @DrawableRes val imageRes: Int,
    @RawRes val soundRes: Int)
