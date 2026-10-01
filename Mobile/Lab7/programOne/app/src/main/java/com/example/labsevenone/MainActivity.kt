package com.example.labsevenone

import android.graphics.Color
import android.graphics.drawable.GradientDrawable
import android.os.Bundle
import android.util.TypedValue
import android.view.View
import android.widget.*
import androidx.activity.ComponentActivity
import androidx.core.content.ContextCompat


class MainActivity : ComponentActivity() {

    private lateinit var preview: View
    private lateinit var rgShape: RadioGroup
    private lateinit var etWidth: EditText
    private lateinit var etHeight: EditText
    private lateinit var etRadius: EditText
    private lateinit var etStroke: EditText
    private lateinit var etColor: EditText
    private lateinit var tvStatus: TextView

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        preview = findViewById(R.id.preview)
        rgShape = findViewById(R.id.rgShape)
        etWidth = findViewById(R.id.etWidth)
        etHeight = findViewById(R.id.etHeight)
        etRadius = findViewById(R.id.etRadius)
        etStroke = findViewById(R.id.etStroke)
        etColor = findViewById(R.id.etColor)
        tvStatus = findViewById(R.id.tvStatus)

        findViewById<Button>(R.id.btnDraw).setOnClickListener { draw() }
    }

    private fun dp(value: Int): Int = TypedValue.applyDimension(
        TypedValue.COMPLEX_UNIT_DIP, value.toFloat(), resources.displayMetrics
    ).toInt()

    /** Читает целое число из поля, при ошибке ставит сообщение в поле. */
    private fun readInt(et: EditText, name: String, min: Int, max: Int, errors: MutableList<String>): Int? {
        val text = et.text.toString().trim()
        val value = text.toIntOrNull()
        val msg = when {
            text.isEmpty() -> "поле пустое"
            value == null -> "нужно целое число"
            value < min || value > max -> "допустимо от $min до $max"
            else -> null
        }
        return if (msg != null) {
            et.error = msg
            errors += "$name: $msg"
            null
        } else value
    }

    private fun draw() {
        val errors = mutableListOf<String>()

        val w = readInt(etWidth, "Ширина", 1, 300, errors)
        val h = readInt(etHeight, "Высота", 1, 300, errors)
        val stroke = readInt(etStroke, "Толщина", 0, 100, errors)

        val shapeRes = when (rgShape.checkedRadioButtonId) {
            R.id.rbOval -> R.drawable.shape_oval
            R.id.rbRing -> R.drawable.shape_ring
            else -> R.drawable.shape_rect
        }
        val isRect = shapeRes == R.drawable.shape_rect
        val isRing = shapeRes == R.drawable.shape_ring

        // Скругление нужно только для прямоугольника
        var radius = 0
        if (isRect) {
            val maxR = if (w != null && h != null) minOf(w, h) / 2 else 150
            radius = readInt(etRadius, "Скругление", 0, maxR, errors) ?: 0
        }

        // Цвет
        var color = Color.BLUE
        try {
            color = Color.parseColor(etColor.text.toString().trim())
        } catch (e: IllegalArgumentException) {
            etColor.error = "формат #RRGGBB"
            errors += "Цвет: неверный формат (пример #FF5722)"
        }

        // Толщина кольца должна быть меньше его радиуса
        if (isRing && w != null && h != null && stroke != null && stroke >= minOf(w, h) / 2) {
            etStroke.error = "для кольца меньше ${minOf(w, h) / 2}"
            errors += "Толщина кольца должна быть меньше радиуса"
        }

        if (errors.isNotEmpty()) {
            tvStatus.setTextColor(Color.RED)
            tvStatus.text = "Ошибка ввода:\n" + errors.joinToString("\n")
            return
        }

        // Все данные корректны: применяем к XML-drawable
        val d = ContextCompat.getDrawable(this, shapeRes)!!.mutate() as GradientDrawable
        d.setColor(color)
        if (isRing) {
            val t = dp(stroke!!)
            d.thickness = t
            d.innerRadius = dp(minOf(w!!, h!!)) / 2 - t
        } else {
            d.setStroke(dp(stroke!!), Color.BLACK)
        }
        if (isRect) d.cornerRadius = dp(radius).toFloat()

        preview.layoutParams = (preview.layoutParams as FrameLayout.LayoutParams).apply {
            width = dp(w!!)
            height = dp(h!!)
        }
        preview.background = d

        tvStatus.setTextColor(Color.parseColor("#2E7D32"))
        tvStatus.text = "Фигура нарисована: ${w}x${h} dp"
    }
}