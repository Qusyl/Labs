package com.example.labsecond8

import android.content.Context
import android.hardware.Sensor
import android.hardware.SensorEvent
import android.hardware.SensorEventListener
import android.hardware.SensorManager
import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.foundation.Canvas
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.material3.Button
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.DisposableEffect
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableIntStateOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.rememberUpdatedState
import androidx.compose.runtime.setValue
import androidx.compose.ui.Modifier
import androidx.compose.ui.geometry.Offset
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.drawscope.Stroke
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.unit.dp
import kotlin.math.min
import kotlin.math.sqrt
import kotlin.random.Random

const val BULBS = 10
const val SHAKE_G = 2.7f
const val SHAKE_PAUSE_MS = 800L

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContent {
            MaterialTheme {
                MagicGarlandScreen()
            }
        }
    }
}


fun randomGarland(): List<Boolean> = List(BULBS) { Random.nextBoolean() }

@Composable
fun MagicGarlandScreen() {
    val context = LocalContext.current

    var winning by remember { mutableStateOf(List(BULBS) { false }) } // выигрышный вариант
    var dropped by remember { mutableStateOf(List(BULBS) { false }) } // выпавший вариант
    var shakes by remember { mutableIntStateOf(0) }


    val roll = {
        winning = randomGarland()
        dropped = randomGarland()
        shakes++
    }
    val currentRoll by rememberUpdatedState(roll)


    DisposableEffect(Unit) {
        val sm = context.getSystemService(Context.SENSOR_SERVICE) as SensorManager
        val accel = sm.getDefaultSensor(Sensor.TYPE_ACCELEROMETER)
        var lastShake = 0L

        val listener = object : SensorEventListener {
            override fun onSensorChanged(e: SensorEvent) {
                val g = sqrt(e.values[0] * e.values[0] +
                        e.values[1] * e.values[1] +
                        e.values[2] * e.values[2]) / SensorManager.GRAVITY_EARTH
                val now = System.currentTimeMillis()
                if (g > SHAKE_G && now - lastShake > SHAKE_PAUSE_MS) {
                    lastShake = now
                    currentRoll()
                }
            }

            override fun onAccuracyChanged(s: Sensor?, accuracy: Int) {}
        }
        if (accel != null) {
            sm.registerListener(listener, accel, SensorManager.SENSOR_DELAY_GAME)
        }
        onDispose { sm.unregisterListener(listener) }
    }

    val matches = winning.indices.count { winning[it] == dropped[it] }

    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(24.dp),
        verticalArrangement = Arrangement.spacedBy(12.dp)
    ) {
        Text("Магическая гирлянда", style = MaterialTheme.typography.headlineMedium)

        if (shakes == 0) {
            Text("Встряхните телефон!")
        } else {
            Text("Встряхиваний: $shakes")
        }

        Text("Выигрышный вариант:", style = MaterialTheme.typography.titleMedium)
        BulbRow(lit = winning)
        Text(winning.toText())

        Text("Выпало случайно:", style = MaterialTheme.typography.titleMedium)
        BulbRow(lit = dropped, compare = if (shakes > 0) winning else null)
        Text(dropped.toText())

        if (shakes > 0) {
            Text(
                "Совпало лампочек: $matches из $BULBS",
                style = MaterialTheme.typography.titleMedium
            )
            Text(
                if (matches == BULBS) "ДЖЕКПОТ! Гирлянды совпали полностью!"
                else "Выигрыша нет, попробуйте ещё раз",
                style = MaterialTheme.typography.titleLarge
            )
        }


        Button(onClick = { roll() }) {
            Text("Встряхнуть (кнопка)")
        }
    }
}


fun List<Boolean>.toText(): String =
    joinToString(" ") { if (it) "1" else "0" }


@Composable
fun BulbRow(lit: List<Boolean>, compare: List<Boolean>? = null) {
    Canvas(
        modifier = Modifier
            .fillMaxWidth()
            .height(56.dp)
    ) {
        val step = size.width / BULBS
        val r = min(step * 0.35f, size.height / 2 * 0.7f)

        for (i in 0 until BULBS) {
            val center = Offset(step * (i + 0.5f), size.height / 2)

            if (lit[i]) {
                drawCircle(Color(0x66FFD54F), r * 1.4f, center)
                drawCircle(Color(0xFFFFD54F), r, center)
            } else {
                drawCircle(Color.Gray, r, center)
            }

            if (compare != null) {
                val ok = lit[i] == compare[i]
                drawCircle(
                    color = if (ok) Color(0xFF2E7D32) else Color(0xFFC62828),
                    radius = r,
                    center = center,
                    style = Stroke(width = 5f)
                )
            }
        }
    }
}