package com.example.background;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;

import android.content.SharedPreferences;
import android.os.AsyncTask;
import android.os.Bundle;
import android.os.Handler;
import android.view.animation.AlphaAnimation;
import android.view.animation.Animation;
import android.widget.TextView;

import com.ramijemli.percentagechartview.PercentageChartView;
import com.ramijemli.percentagechartview.callback.ProgressTextFormatter;

import org.json.JSONArray;
import org.json.JSONObject;

import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.URL;

import javax.net.ssl.HttpsURLConnection;

public class MainActivity extends AppCompatActivity {
    String url = "https://api.asksensors.com/read/Y4tLWrhgwCtpIgHHQvsJgT9cIebkCBhb?module=module1&1=number_of_entiries";
    String url3 = "https://api.asksensors.com/read/Y4tLWrhgwCtpIgHHQvsJgT9cIebkCBhb?module=module3&1=number_of_entiries";
    String url2 = "https://api.asksensors.com/read/Y4tLWrhgwCtpIgHHQvsJgT9cIebkCBhb?module=module2&1=number_of_entiries";
    Handler handler = new Handler();
    Runnable runnable;
    private PercentageChartView temp, led, hum;
    TextView situation;
    TextView stopwatch;
    TextView date;
    String open;
    String dates[];
    String lastDate;
    int situations;
    int preValue;
    int counter = 1;


    private AlphaAnimation animation1 = new AlphaAnimation(0.1f, 1.0f);

    long Seconds, Minutes, Hours, start;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);


        temp = findViewById(R.id.temp);
        led = findViewById(R.id.led);
        hum = findViewById(R.id.humidity);
        situation = findViewById(R.id.situation);
        stopwatch = findViewById(R.id.stopwatch);
        date = findViewById(R.id.date);

        try {

            Situation situation = new Situation();
            situation.execute(url3);
            Temperature temperature = new Temperature();
            temperature.execute(url);
            Humidity humidity = new Humidity();
            humidity.execute(url2);


        } catch (Exception e) {

        }

        ProgressTextFormatter progressTextFormatter = new ProgressTextFormatter() {
            @NonNull
            @Override
            public CharSequence provideFormattedText(float progress) {
                int temp = (int) (progress / 2);
                return temp + " °C";

            }
        };
        temp.setTextFormatter(progressTextFormatter);


        ProgressTextFormatter progressTextFormatter3 = new ProgressTextFormatter() {
            @NonNull
            @Override
            public CharSequence provideFormattedText(float progress) {

                return "";
            }
        };
        led.setTextFormatter(progressTextFormatter3);

        ProgressTextFormatter progressTextFormatter2 = new ProgressTextFormatter() {
            @NonNull
            @Override
            public CharSequence provideFormattedText(float progress) {
                int hum = (int) (progress);
                return hum + " %";

            }
        };

        hum.setTextFormatter(progressTextFormatter2);

    }


    private void malik(long start) {

        runnable = new Runnable() {
            @Override
            public void run() {
                Seconds = (System.currentTimeMillis() - start) / 1000;

                Hours = Seconds / 3600;
                Minutes = (Seconds % 3600) / 60;
                Seconds = Seconds % 60;

                stopwatch.setText(String.format("%02d", Hours) + ":"
                        + String.format("%02d", Minutes) + ":"
                        + String.format("%02d", Seconds));
                handler.postDelayed(this, 1000);
            }
        };
        handler.post(runnable);


    }


    private class Situation extends AsyncTask<String, Void, String> {

        @Override
        protected String doInBackground(String... strings) {


            String result = "";
            URL url3;
            HttpsURLConnection httpsURLConnection;
            try {

                url3 = new URL("https://api.asksensors.com/read/Y4tLWrhgwCtpIgHHQvsJgT9cIebkCBhb?module=module3&1=number_of_entiries");
                httpsURLConnection = (HttpsURLConnection) url3.openConnection();
                InputStream inputStream = httpsURLConnection.getInputStream();
                InputStreamReader inputStreamReader = new InputStreamReader(inputStream);

                int data = inputStream.read();
                while (data > 0) {
                    char character = (char) data;
                    result += character;
                    data = inputStream.read();
                }
            } catch (Exception e) {
                return null;
            }
            return result;

        }

        @Override
        protected void onPostExecute(String s) {
            super.onPostExecute(s);
            try {

                JSONArray jsonArray = new JSONArray(s);
                JSONObject jsonObject = jsonArray.getJSONObject(0);
                open = jsonObject.getString("value");
                JSONObject jsonObject1 = jsonArray.getJSONObject(0);
                dates = new  String[]{jsonObject1.getString("date")};
                lastDate = dates[0];
                dates[0] = lastDate;
                SharedPreferences sharedPreferences = getSharedPreferences("prefs", MODE_PRIVATE);

                System.out.println(open);
                situations = Integer.parseInt(open);
                if (situations != preValue) {
                    preValue = situations;
                    if (situations == 1) {
                        start = sharedPreferences.getLong("start", 0);
                        date.setText(lastDate.substring(0, 19));
                        if (start == 0) {
                            SharedPreferences.Editor editor = sharedPreferences.edit();
                            start = System.currentTimeMillis();
                            editor.putLong("start", start);
                            editor.apply();
                        }
                        malik(start);
                        animation1.setRepeatCount(Animation.INFINITE);
                        animation1.setRepeatMode(Animation.REVERSE);
                        animation1.setDuration(1000);
                        animation1.setStartOffset(100);
                        animation1.setFillAfter(true);
                        led.startAnimation(animation1);
                        situation.setText("Kapak açık!");

                    } else {
                        handler.removeCallbacks(runnable);
                        led.clearAnimation();
                        led.setAlpha(1);
                        situation.setText("Kapak kapalı.");
                        stopwatch.setText("00:00:00");
                        SharedPreferences.Editor editor = sharedPreferences.edit();
                        editor.putLong("start", 0);
                        editor.apply();
                    }


                }


            } catch (Exception e) {

            }

            (new Handler()).postDelayed(MainActivity.this::executeStuation, 5000);
            if (situations != counter) {
                counter = situations;
                if (situations == 0) {
                    date.setText(lastDate.substring(0,19));
                }
            }
        }

    }

    private void executeStuation() {

        Situation stuation = new Situation();
        stuation.execute(url3);
    }

    private class Temperature extends AsyncTask<String, Void, String> {

        @Override
        protected String doInBackground(String... strings) {


            String result = "";
            URL url;
            HttpsURLConnection httpsURLConnection;
            try {

                url = new URL(strings[0]);
                httpsURLConnection = (HttpsURLConnection) url.openConnection();
                InputStream inputStream = httpsURLConnection.getInputStream();
                InputStreamReader inputStreamReader = new InputStreamReader(inputStream);

                int data = inputStream.read();
                while (data > 0) {
                    char character = (char) data;
                    result += character;
                    data = inputStream.read();
                }
            } catch (Exception e) {
                return null;
            }
            return result;


        }

        @Override
        protected void onPostExecute(String s) {

            super.onPostExecute(s);

            try {

                //System.out.println("alınan data " +s);
                JSONArray jsonArray = new JSONArray(s);
                JSONObject jsonObject = jsonArray.getJSONObject(0);
                open = jsonObject.getString("value");


                Float f = Float.parseFloat(open);
                temp.setProgress(f * 2, true);


            } catch (Exception e) {

            }

            (new Handler()).postDelayed(MainActivity.this::executeTemprature, 5000);

        }
    }

    private void executeTemprature() {

        Temperature temperature = new Temperature();
        temperature.execute(url);

    }


    private class Humidity extends AsyncTask<String, Void, String> {

        @Override
        protected String doInBackground(String... strings) {


            String result = "";
            URL url2;
            HttpsURLConnection httpsURLConnection;
            try {

                url2 = new URL(strings[0]);
                httpsURLConnection = (HttpsURLConnection) url2.openConnection();
                InputStream inputStream = httpsURLConnection.getInputStream();
                InputStreamReader inputStreamReader = new InputStreamReader(inputStream);

                int data = inputStream.read();
                while (data > 0) {
                    char character = (char) data;
                    result += character;
                    data = inputStream.read();
                }
            } catch (Exception e) {
                return null;
            }
            return result;

        }

        @Override
        protected void onPostExecute(String s) {
            super.onPostExecute(s);

            try {

                //System.out.println("alınan data " +s);
                JSONArray jsonArray = new JSONArray(s);
                for (int i = jsonArray.length() - 1; i > 0; i--) {
                    JSONObject jsonObject = jsonArray.getJSONObject(i);
                    open = jsonObject.getString("value");
                }

                Float h = Float.parseFloat(open);
                hum.setProgress(h, true);

            } catch (Exception e) {

            }
            (new Handler()).postDelayed(MainActivity.this::executeHumidity, 5000);
        }

    }


    private void executeHumidity() {
        Humidity humidity = new Humidity();
        humidity.execute(url2);

    }
}





