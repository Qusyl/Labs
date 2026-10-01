package com.example.firstlab8

import android.content.Context
import android.hardware.Sensor
import android.hardware.SensorEvent
import android.hardware.SensorEventListener
import android.hardware.SensorManager
import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.padding
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.DisposableEffect
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.unit.dp

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContent {
            MaterialTheme {
                RotationVectorScreen()
            }
        }
    }
}

@Composable
fun RotationVectorScreen() {
    val context = LocalContext.current


    val sensorManager = remember {
        context.getSystemService(Context.SENSOR_SERVICE) as SensorManager
    }
    val sensor = remember {
        sensorManager.getDefaultSensor(Sensor.TYPE_ROTATION_VECTOR)
    }


    var values by remember { mutableStateOf(FloatArray(4)) }


    DisposableEffect(sensor) {
        val listener = object : SensorEventListener {
            override fun onSensorChanged(event: SensorEvent) {

                values = event.values.copyOf()
            }

            override fun onAccuracyChanged(sensor: Sensor?, accuracy: Int) {}
        }
        if (sensor != null) {
            sensorManager.registerListener(
                listener, sensor, SensorManager.SENSOR_DELAY_UI
            )
        }
        onDispose { sensorManager.unregisterListener(listener) }
    }

    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(24.dp),
        verticalArrangement = Arrangement.spacedBy(12.dp)
    ) {
        Text("Ориентация устройства (вектор поворота)", style = MaterialTheme.typography.titleLarge)

        if (sensor == null) {
            Text("Датчик TYPE_ROTATION_VECTOR на этом устройстве отсутствует")
        } else {
            Text("Вектор поворота (без единиц):", style = MaterialTheme.typography.titleMedium)
            Text("x = %.4f".format(values[0]))
            Text("y = %.4f".format(values[1]))
            Text("z = %.4f".format(values[2]))
            Text("cos(θ/2) = %.4f".format(values.getOrElse(3) { 0f }))
        }
    }
}