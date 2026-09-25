# AGENTS.md

## Project

The application lives in `mobile/` and follows the stack documented in `README.md`:
React Native, Expo, TypeScript, React Navigation, Zustand, Axios, Expo Secure Store,
and Expo Notifications for the native FCM device token.

## Commands

Run commands from `mobile/`:

- `npm install` installs dependencies.
- `npm start` starts the Expo development server.
- `npm run android` starts the Android target.
- `npm run ios` starts the iOS target (requires macOS for a native build).
- `npm run web` starts the web target.
- `npx tsc --noEmit` checks TypeScript.
- `npx expo-doctor` validates Expo configuration and dependency versions.

Set `EXPO_PUBLIC_API_URL` to the ASP.NET Core API base URL. The fallback is
`http://localhost:5000/api`; Android emulators may need `http://10.0.2.2:5000/api`.

## Structure

- `mobile/src/navigation/` owns auth, role-based navigation, and route types.
- `mobile/src/screens/` owns auth, parent, student, and shared screens.
- `mobile/src/services/` owns Axios/API integrations, authentication, payments, and notifications.
- `mobile/src/stores/` owns Zustand state for auth, parent, and student flows.
- `mobile/src/types/` owns API/domain contracts.

The demo buttons on the login screen intentionally create a local session for UI work without a backend. Production login uses `POST /auth/login` and persists the JWT in Expo Secure Store.

Use this file to document commands and conventions only after they can be verified from the repo.
