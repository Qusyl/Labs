package com.example.myapplication

import android.graphics.Color
import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.padding
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Brush
import androidx.compose.ui.text.font.FontStyle
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.example.myapplication.ui.theme.PurpleGradient;
import com.example.myapplication.ui.theme.PinkGradient;
import com.example.myapplication.ui.theme.BluePurpleGradient;
import com.example.myapplication.ui.theme.BlueGradient;

import com.example.myapplication.ui.theme.MyApplicationTheme

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContent {
            MyApplicationTheme {
                Scaffold(modifier = Modifier.fillMaxSize()) { innerPadding ->

                    Greeting(
                        name = "World",
                        modifier = Modifier
                            .padding(innerPadding)
                            .fillMaxSize()
                    )

                }
            }
        }
    }
}

@Composable
fun Greeting(name: String, modifier: Modifier = Modifier.fillMaxSize()) {
    GreetingBackground(modifier, name)


}

@Composable
fun GreetingBackground(modifier: Modifier = Modifier, name: String){
    var colors = listOf<androidx.compose.ui.graphics.Color>(PurpleGradient, PinkGradient, BluePurpleGradient, BlueGradient )
    var gradient = Brush.linearGradient(colors = colors)

    Box(modifier = modifier.background(gradient), contentAlignment = Alignment.Center){
        Text(
            text = "Hello $name!",
            fontStyle = FontStyle.Italic,
            fontWeight = FontWeight.Bold,
            fontSize = 48.sp
        )
    }
}