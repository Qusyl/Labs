package com.example.labseventwo.views

import android.content.Context
import android.graphics.*
import android.util.AttributeSet
import android.view.View

class FloppyView(context: Context, attrs: AttributeSet? = null) : View(context, attrs) {

    private var bitmap: Bitmap? = null
    private val paint = Paint(Paint.ANTI_ALIAS_FLAG)

    override fun onSizeChanged(w: Int, h: Int, oldw: Int, oldh: Int) {
        super.onSizeChanged(w, h, oldw, oldh)
        if (w == 0 || h == 0) return


        val bmp = Bitmap.createBitmap(w, h, Bitmap.Config.ARGB_8888)
        val c = Canvas(bmp)
        c.drawColor(Color.parseColor("#ECEFF1"))


        val s = minOf(w, h) / 300f
        c.translate((w - 300 * s) / 2, (h - 300 * s) / 2)
        c.scale(s, s)

        drawFloppy(c)
        bitmap = bmp
    }

    private fun drawFloppy(c: Canvas) {

        val body = Path().apply {
            moveTo(0f, 0f)
            lineTo(260f, 0f)
            lineTo(300f, 40f)
            lineTo(300f, 300f)
            lineTo(0f, 300f)
            close()
        }
        paint.pathEffect = CornerPathEffect(8f)
        fill(c, body, "#1E3A8A")
        stroke(c, body, "#0B1B4D", 4f)
        paint.pathEffect = null


        rect(c, 55f, 0f, 235f, 85f, "#B0BEC5")
        strokeRect(c, 55f, 0f, 235f, 85f, "#546E7A", 3f)

        rect(c, 165f, 12f, 205f, 72f, "#263238")


        rect(c, 35f, 140f, 265f, 285f, "#FAFAFA")
        rect(c, 35f, 140f, 265f, 172f, "#E53935")

        for (y in listOf(200f, 227f, 254f)) {
            paint.style = Paint.Style.STROKE
            paint.strokeWidth = 2f
            paint.color = Color.parseColor("#90A4AE")
            c.drawLine(50f, y, 250f, y, paint)
        }

        paint.style = Paint.Style.FILL
        paint.color = Color.WHITE
        paint.textSize = 22f
        paint.typeface = Typeface.DEFAULT_BOLD
        c.drawText("MY DISK 1.44MB", 50f, 164f, paint)

        paint.color = Color.parseColor("#1565C0")
        paint.textSize = 20f
        paint.typeface = Typeface.DEFAULT
        c.drawText("Лабораторная работа", 52f, 196f, paint)


        rect(c, 12f, 262f, 30f, 280f, "#90CAF9")
        strokeRect(c, 12f, 262f, 30f, 280f, "#0B1B4D", 2f)


        paint.style = Paint.Style.FILL
        paint.color = Color.parseColor("#0B1B4D")
        c.drawCircle(270f, 15f, 0.1f, paint)
    }


    private fun rect(c: Canvas, l: Float, t: Float, r: Float, b: Float, color: String) {
        paint.style = Paint.Style.FILL
        paint.color = Color.parseColor(color)
        c.drawRect(l, t, r, b, paint)
    }

    private fun strokeRect(c: Canvas, l: Float, t: Float, r: Float, b: Float, color: String, w: Float) {
        paint.style = Paint.Style.STROKE
        paint.strokeWidth = w
        paint.color = Color.parseColor(color)
        c.drawRect(l, t, r, b, paint)
    }

    private fun fill(c: Canvas, p: Path, color: String) {
        paint.style = Paint.Style.FILL
        paint.color = Color.parseColor(color)
        c.drawPath(p, paint)
    }

    private fun stroke(c: Canvas, p: Path, color: String, w: Float) {
        paint.style = Paint.Style.STROKE
        paint.strokeWidth = w
        paint.color = Color.parseColor(color)
        c.drawPath(p, paint)
    }

    override fun onDraw(canvas: Canvas) {
        bitmap?.let { canvas.drawBitmap(it, 0f, 0f, null) }
    }
}