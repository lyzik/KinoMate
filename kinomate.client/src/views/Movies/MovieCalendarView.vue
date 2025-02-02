<template>
    <div class="mx-10">
      <v-container class="mt-16" style="max-width: 2000px;">
        <FullCalendar class="mt-5" :options="calendarOptions" />
      </v-container>
    </div>
  </template>
  
  <script>
  import axios from "axios";
  import FullCalendar from "@fullcalendar/vue3";
  import dayGridPlugin from "@fullcalendar/daygrid";

  export default {
    name: "MovieCalendarView",
    components: { FullCalendar },
    data() {
      return {
        month: new Date().getMonth(),
        year: new Date().getFullYear(),
        movies: [],
        calendarOptions: {
          plugins: [dayGridPlugin],
          initialView: "dayGridMonth",
          events: [],
          eventClick: this.handleEventClick,
          datesSet: this.handleDatesSet,
        },
      };
    },
    mounted() {
      this.fetchUpcomingMovies();
    },
    methods: {
      async fetchUpcomingMovies() {
        try {
          const response = await axios.get(
            `/api/fetch/upcomingMovies/${this.year}/${this.month + 1}`
          );
          this.movies = response.data;
          const events = this.movies.map((movie) => ({
            id: movie.id,
            title: movie.title,
            date: movie.release_date,
          }));
          this.calendarOptions = {
            ...this.calendarOptions,
            events,
          };
        } catch (error) {
          console.error("Error fetching upcoming movies:", error);
        }
      },
      handleEventClick(info) {
        this.goToMovie(info.event.id);
      },
      goToMovie(movieId) {
        this.$router.push(`/movies/${movieId}`);
      },
      handleDatesSet(info) {
        this.year = info.view.currentStart.getFullYear();
        this.month = info.view.currentStart.getMonth();
        this.fetchUpcomingMovies();
      },
    },
  };
  </script>