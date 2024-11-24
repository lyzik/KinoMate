export const emailRules = [
  (v) => !!v || "Email is required",
  (v) => /.+@.+\..+/.test(v) || "E-mail must be valid",
];

export const usernameRules = [
  (v) => !!v || "Username is required",
  (v) => v.length >= 3 || "Username must be at least 3 characters",
(v) => !/\s/.test(v) || "Username must not contain spaces",
];

export const passwordRules = [
  (v) => !!v || "Password is required",
  (v) => v.length >= 8 || "Password must be at least 8 characters long",
  (v) =>
    /[A-Z]/.test(v) || "Password must contain at least one uppercase letter",
  (v) =>
    /[a-z]/.test(v) || "Password must contain at least one lowercase letter",
  (v) => /[0-9]/.test(v) || "Password must contain at least one number",
  (v) =>
    /[\W_]/.test(v) || "Password must contain at least one special character",
];

export const confirmPasswordRule = (password) => {
  return (v) => v === password || "Passwords do not match";
};
